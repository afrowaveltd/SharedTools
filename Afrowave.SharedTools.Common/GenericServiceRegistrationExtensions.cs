using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Afrowave.SharedTools.Common
{
   /// <summary>
   /// Extension helpers to register services together with strongly-typed options using a consistent API.
   /// </summary>
   public static class GenericServiceRegistrationExtensions
   {
      /// <summary>
      /// Registers <typeparamref name="TImplementation"/> as <typeparamref name="TService"/> and configures
      /// <typeparamref name="TOptions"/> using the provided configuration action.
      /// </summary>
      /// <remarks>
      /// The implementation constructor may accept either <typeparamref name="TOptions"/>,

      /// <see cref="IOptions{TOptions}"/>, or <see cref="IOptionsMonitor{TOptions}"/>.
      /// </remarks>
      /// <typeparam name="TService">The service abstraction to register.</typeparam>
      /// <typeparam name="TImplementation">The concrete implementation of the service.</typeparam>
      /// <typeparam name="TOptions">The options type to be added to the options pipeline.</typeparam>
      /// <param name="services">The DI service collection.</param>
      /// <param name="configure">A delegate used to configure the options instance.</param>
      /// <param name="lifetime">The desired service lifetime. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
      /// <param name="validate">Optional validation predicate for the options.</param>
      /// <param name="validationMessage">Validation failure message used when <paramref name="validate"/> returns <see langword="false"/>.</param>
      /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
      /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
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
      /// Registers <typeparamref name="TImplementation"/> as <typeparamref name="TService"/> and binds
      /// <typeparamref name="TOptions"/> from the provided configuration section.
      /// </summary>
      /// <remarks>
      /// The options are bound using <see cref="ConfigurationBinder"/>.
      /// </remarks>
      /// <typeparam name="TService">The service abstraction to register.</typeparam>
      /// <typeparam name="TImplementation">The concrete implementation of the service.</typeparam>
      /// <typeparam name="TOptions">The options type to be bound from configuration.</typeparam>
      /// <param name="services">The DI service collection.</param>
      /// <param name="section">The configuration section containing the options data.</param>
      /// <param name="lifetime">The desired service lifetime. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
      /// <param name="validate">Optional validation predicate for the options.</param>
      /// <param name="validationMessage">Validation failure message used when <paramref name="validate"/> returns <see langword="false"/>.</param>
      /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
      /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="section"/> is <see langword="null"/>.</exception>
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

         // Bind using ConfigurationBinder without requiring the OptionsBuilder Bind extension
         var builder = services.AddOptions<TOptions>();
         builder.Configure(opts => Microsoft.Extensions.Configuration.ConfigurationBinder.Bind(section, opts));

         if(validate != null)
            builder.Validate(validate, validationMessage);

         RegisterService<TService, TImplementation>(services, lifetime);
         return services;
      }

      /// <summary>
      /// Adds a named options profile for <typeparamref name="TOptions"/> while ensuring the service is registered once.
      /// Implementations can resolve <see cref="IOptionsMonitor{TOptions}"/> and call <c>Get(name)</c> to access the profile.
      /// </summary>
      /// <typeparam name="TService">The service abstraction to register.</typeparam>
      /// <typeparam name="TImplementation">The concrete implementation of the service.</typeparam>
      /// <typeparam name="TOptions">The options type to configure.</typeparam>
      /// <param name="services">The DI service collection.</param>
      /// <param name="name">The options profile name.</param>
      /// <param name="configure">A delegate used to configure the named options instance.</param>
      /// <param name="lifetime">The desired service lifetime. Defaults to <see cref="ServiceLifetime.Singleton"/>.</param>
      /// <returns>An <see cref="OptionsBuilder{TOptions}"/> for further configuration of the named options.</returns>
      /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/>, <paramref name="name"/>, or <paramref name="configure"/> is <see langword="null"/>.</exception>
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

         // Register the service once (idempotent — multiple calls are safe)
         RegisterService<TService, TImplementation>(services, lifetime);

         return services.AddOptions<TOptions>(name).Configure(configure);
      }

      /// <summary>
      /// Centralized helper to register the service mapping with the requested lifetime.
      /// </summary>
      private static void RegisterService<TService, TImplementation>(
          IServiceCollection services, ServiceLifetime lifetime)
          where TService : class
          where TImplementation : class, TService
      {
         // Unified registration for all lifetimes
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
