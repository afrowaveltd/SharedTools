using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Docs.Services
{
	public class HttpService
	{
		private LibreTranslateOptions _serverOptions = new();
		private HttpClient _client = new HttpClient();
		private string _baseUri;
		private string _apiKey;
		private bool _needsKey;
		private string _languagesEndpoint;
		private string _translateEndpoint;
		private string _translateFileEndpoint;
		private string _detectLanguageEndpoint;

		private readonly JsonSerializerOptions _options = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		public HttpService(IConfiguration configuration, HttpClient client)
		{
			LibreTranslateOptions o = configuration.GetSection("LibreTranslateOptions").Get<LibreTranslateOptions>() ?? new();
			_baseUri = o.Host;
			_apiKey = o.ApiKey;
			_needsKey = o.NeedsKey;
			_languagesEndpoint = o.LanguagesEndpoint;
			_translateEndpoint = o.TranslateEndpoint;
			_translateFileEndpoint = o.TranslateFileEndpoint;
			_detectLanguageEndpoint = o.DetectLanguageEndpoint;

			if(o.NeedsKey)
			{
				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + o.ApiKey);
			}
			client.BaseAddress = new Uri(o.Host);
			_client = client;
		}
	}
}