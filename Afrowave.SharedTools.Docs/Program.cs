using Afrowave.SharedTools.Docs.I18n;
using Afrowave.SharedTools.Docs.Middlewares;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddOpenApi("AfrowaveId");
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

// transient services
builder.Services.AddTransient<IStringLocalizerFactory, JsonStringLocalizerFactory>();

// standalone services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ILanguageService, LanguageService>();
builder.Services.AddSingleton<HttpClient>();
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

app.UseRouting();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
	.WithStaticAssets();

app.Run();