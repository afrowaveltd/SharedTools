using Afrowave.SharedTools.Docs.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(builder.Configuration)
   .CreateBootstrapLogger();
try
{
   builder.Logging.ClearProviders();

   builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

   // Add Application database context
   builder.Services.AddDbContext<DocsDbContext>(options =>
   {
      options.UseSqlite(
         builder.Configuration.GetConnectionString("DefaultConnection"));
   });
   // Add services to the container.
   builder.Services.AddHttpContextAccessor();
   builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
   {
      // Set property naming policy to camelCase
      options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

      // Allow complex object types like Lists<T> or other nested members
      options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;

      // Add support for preserving references if needed (useful for circular references)
      options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

      // Customize any other settings as needed (e.g., number or date handling)
   });

   builder.Services
       .AddLocalization();

   builder.Services.AddOpenApi("Afrowave");
   builder.Services.AddSignalR()
      .AddJsonProtocol(options =>
      {
         // Set property naming policy to camelCase
         options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
         // Allow complex object types like Lists<T> or other nested members
         options.PayloadSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
         // Add support for preserving references if needed (useful for circular references)
         options.PayloadSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
      });
   builder.Services.AddRazorPages()
      .AddViewLocalization();

   builder.Services.AddControllers()
             .AddJsonOptions(options =>
             {
                // Set property naming policy to camelCase
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                // Allow Lists and nested objects
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;

                // Handle circular references if applicable
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
             })
             .AddXmlDataContractSerializerFormatters();

   // middlewares
   builder.Services.AddTransient<I18nMiddleware>();

   // scoped services
   builder.Services.AddScoped<ICookieService, CookieService>();
   builder.Services.AddScoped<IOptionsService, OptionsService>();

   // transient services
   builder.Services.AddTransient<IStringLocalizerFactory, JsonStringLocalizerFactory>();
   builder.Services.AddSingleton<ILibreFileService, LibreFileService>();
   builder.Services.AddScoped<ICyclicTranslationService, CyclicTranslationService>();

   // standalone services
   builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
   builder.Services.AddSingleton<ILanguageService, LanguageService>();
   builder.Services.AddSingleton<ILibreTranslateService, LibreTranslateService>();
   builder.Services.AddSingleton<IHttpService, HttpService>();

   // hosted services
   builder.Services.AddHostedService<TranslatorHostedService>();

   Log.Information("Services loaded");
}
catch(Exception ex)
{
   Log.Fatal(ex, "Application start-up failed");
   throw;
}
finally
{
   Log.CloseAndFlush();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

string[] supportedCultures = ["en"];

ILanguageService languageService = app.Services.GetRequiredService<ILanguageService>();
if(languageService != null)
{
   supportedCultures = languageService.TranslationsPresented();
}

app.UseMiddleware<I18nMiddleware>();
app.UseRequestLocalization(options =>
{
   options.AddSupportedCultures(supportedCultures)
       .AddSupportedUICultures(supportedCultures)
       .SetDefaultCulture("en")
       .ApplyCurrentCultureToResponseHeaders = true;
});
app.MapOpenApi()
   .CacheOutput();

app.MapScalarApiReference(options =>
{
   options
     .WithTitle("Afrowave Documentations")
     .WithTheme(ScalarTheme.Mars)
     .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});
app.Use(async (context, next) =>
{
   if(context.Request.Path == "/api" || context.Request.Path == "/api/")
   {
      context.Response.Redirect("/scalar/afrowave");
      return;
   }

   await next();
});

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapHub<AdminHub>("/admin_hub");
app.MapHub<OpenHub>("/open_hub");
app.MapHub<RealtimeHub>("/realtime_hub");

app.Run();