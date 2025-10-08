using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Afrowave.SharedTools.Common
{
   public static class GenericServiceRegistrationExtensions
   {
      /// <summary>
      /// Zaregistruje implementaci TImplementation jako TService a nakonfiguruje TOptions přes lambda.
      /// TImplementation může v konstruktoru přijímat buď TOptions nebo IOptions&lt;TOptions&gt; / IOptionsMonitor&lt;TOptions&gt;.
      /// </summary>
      public static IServiceCollection AddConfiguredService<TService, TImplementation, TOptions>(
          this IServiceCollection services,
          System.Action<TOptions> configure,
          ServiceLifetime lifetime = ServiceLifetime.Singleton,
          Func<TOptions, bool>? validate = null,
          string validationMessage = "Invalid options")
          where TService : class
          where TImplementation : class, TService
          where TOptions : class, new()
      {
         if(services == null) throw new ArgumentNullException(nameof(services));
         if(configure == null) throw new ArgumentNullException(nameof(configure));

         var builder = services.AddOptions<TOptions>().Configure(configure);

         if(validate != null)
            builder.Validate(validate, validationMessage);

         RegisterService<TService, TImplementation>(services, lifetime);
         return services;
      }

      /// <summary>
      /// Zaregistruje službu a nabinduje TOptions ze zadané konfigurace (např. builder.Configuration.GetSection("MyService")).
      /// </summary>
      public static IServiceCollection AddConfiguredService<TService, TImplementation, TOptions>(
          this IServiceCollection services,
          IConfiguration section,
          ServiceLifetime lifetime = ServiceLifetime.Singleton,
          Func<TOptions, bool>? validate = null,
          string validationMessage = "Invalid options")
          where TService : class
          where TImplementation : class, TService
          where TOptions : class, new()
      {
         if(services == null) throw new ArgumentNullException(nameof(services));
         if(section == null) throw new ArgumentNullException(nameof(section));

         // Bind pomocí ConfigurationBinder bez nutnosti rozšiřující metody Bind na OptionsBuilder
         var builder = services.AddOptions<TOptions>();
         builder.Configure(opts => Microsoft.Extensions.Configuration.ConfigurationBinder.Bind(section, opts));

         if(validate != null)
            builder.Validate(validate, validationMessage);

         RegisterService<TService, TImplementation>(services, lifetime);
         return services;
      }

      /// <summary>
      /// Named options profil (víc konfigurací pro stejnou službu). Implementace si načte IOptionsMonitor&lt;TOptions&gt;.Get(name).
      /// </summary>
      public static OptionsBuilder<TOptions> AddServiceProfile<TService, TImplementation, TOptions>(
          this IServiceCollection services,
          string name,
          System.Action<TOptions> configure,
          ServiceLifetime lifetime = ServiceLifetime.Singleton)
          where TService : class
          where TImplementation : class, TService
          where TOptions : class, new()
      {
         if(services == null) throw new ArgumentNullException(nameof(services));
         if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
         if(configure == null) throw new ArgumentNullException(nameof(configure));

         // samotnou službu zaregistrujeme jen jednou (idempotentní — nevadí více volání)
         RegisterService<TService, TImplementation>(services, lifetime);

         return services.AddOptions<TOptions>(name).Configure(configure);
      }

      private static void RegisterService<TService, TImplementation>(
          IServiceCollection services, ServiceLifetime lifetime)
          where TService : class
          where TImplementation : class, TService
      {
         // jednotná registrace pro všechny lifetime
         switch(lifetime)
         {
            case ServiceLifetime.Singleton:
               services.AddSingleton<TService, TImplementation>();
               break;
            case ServiceLifetime.Scoped:
               services.AddScoped<TService, TImplementation>();
               break;
            default:
               services.AddTransient<TService, TImplementation>();
               break;
         }
      }
   }
}
