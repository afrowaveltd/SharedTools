using Afrowave.SharedTools.Models.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.LibreTranslate.Services
{
	public class LanguageService
	{
		private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		private readonly string filePath = Path.Combine(AppContext.BaseDirectory, "Jsons", "languages.json");
		private readonly List<Language> _languages = new List<Language>();

		public LanguageService()
		{
			if(!File.Exists(filePath))
			{
				throw new FileNotFoundException($"The file at path {filePath} was not found.");
			}

			if(File.Exists(filePath))
			{
				try
				{
					var json = File.ReadAllText(filePath);
					_languages = JsonSerializer.Deserialize<List<Language>>(json, _options) ?? new List<Language>();
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException("Failed to load languages from the JSON file.", ex);
				}
			}
		}

		private List<Language> GetLanguages()
		{
			var response = new List<Language>(_languages);
			return response;
		}
	}
}