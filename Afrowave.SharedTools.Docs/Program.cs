using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
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

_ = builder.Services
	 .AddLocalization();

builder.Services.AddScoped<ICookieService, CookieService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
	.WithStaticAssets();

app.Run();