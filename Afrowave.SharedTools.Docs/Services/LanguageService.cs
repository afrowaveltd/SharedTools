﻿using System.Text.Json;

namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides functionality for managing and retrieving information about supported languages.
	/// </summary>
	/// <remarks>The <see cref="LanguageService"/> class maintains a predefined list of languages, each represented
	/// by a <see cref="Language"/> object. This list includes details such as the language code, name, native name, and
	/// whether the language is written in a right-to-left (RTL) script. Use this class to access the list of supported
	/// languages and their metadata.</remarks>
	public class LanguageService : ILanguageService
	{
		private static readonly string _localesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0], "Locales");
		private static readonly string _oldTranslationPath = Path.Combine(Path.GetTempPath(), "old.json");

		/// <summary>
		/// Retrieves the list of available languages.
		/// </summary>
		/// <returns>A list of <see cref="Language"/> objects representing the available languages. If no languages are available, the
		/// list will be empty.</returns>
		public List<Language> GetLanguages() => _languages;

		/// <summary>
		/// Determines whether the specified language code corresponds to a right-to-left (RTL) script.
		/// </summary>
		/// <param name="code">The language code to check.</param>
		/// <returns>True if the language is written in RTL; otherwise, false.</returns>
		public bool IsRtl(string code)
		{
			if(string.IsNullOrWhiteSpace(code))
			{
				return false;
			}
			var language = _languages.FirstOrDefault(l => l.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
			return language?.Rtl ?? false;
		}

		/// <summary>
		/// Retrieves a language by its code.
		/// </summary>
		/// <remarks>The search is case-insensitive and matches the language code using <see
		/// cref="StringComparison.OrdinalIgnoreCase"/>.</remarks>
		/// <param name="code">The language code to search for. This parameter cannot be null, empty, or consist solely of whitespace.</param>
		/// <returns>A <see cref="Response{Language}"/> object containing the language associated with the specified code if found. If
		/// the code is invalid or no matching language is found, the response will indicate failure with an appropriate error
		/// message.</returns>
		public Response<Language>? GetLanguageByCode(string code)
		{
			Response<Language> result = new();
			if(string.IsNullOrWhiteSpace(code))
			{
				Response<Language>.Fail("Language code cannot be null or empty.");
			}
			var language = _languages.FirstOrDefault(l => l.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
			if(language == null)
			{
				return Response<Language>.Fail($"Language with code '{code}' not found.");
			}
			return Response<Language>.Successful(language, "Language found successfully.");
		}

		/// <summary>
		/// Retrieves the required languages asynchronously.
		/// </summary>
		/// <returns>A response containing a list of required languages.</returns>
		public Response<List<Language>> GetRequiredLanguagesAsync()
		{
			try
			{
				if(!Directory.Exists(_localesPath))
				{
					return Response<List<Language>>.Fail("Locales directory not found");
				}

				var languageFiles = Directory.GetFiles(_localesPath, "??.json"); // Pouze 2 znaky před .json
				List<Language> requiredLanguages = [];

				foreach(var file in languageFiles)
				{
					var languageCode = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();

					// Dodatečná validace - pouze písmena, přesně 2 znaky
					if(languageCode.Length == 2 && languageCode.All(char.IsLetter))
					{
						var language = _languages.FirstOrDefault(l =>
							 l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase));

						if(language != null)
						{
							requiredLanguages.Add(language);
						}
					}
				}

				return Response<List<Language>>.Successful(requiredLanguages, $"Successfully retrieved {requiredLanguages.Count} languages");
			}
			catch(Exception ex)
			{
				return Response<List<Language>>.Fail($"Error retrieving languages: {ex.Message}");
			}
		}

		/// <summary>
		/// Asynchronously retrieves a dictionary of key-value pairs from a JSON file based on the specified locale code.
		/// </summary>
		/// <remarks>The method looks for a JSON file in the configured locales directory, with the file name matching
		/// the lowercase version of the locale code. If the file is found and successfully deserialized, the resulting
		/// dictionary is returned.  If the file is empty or contains no data, a warning is included in the
		/// response.</remarks>
		/// <param name="code">A two-character locale code (e.g., "en" for English or "fr" for French). The code must be non-null and exactly two
		/// characters long.</param>
		/// <returns>A <see cref="Response{T}"/> containing the dictionary of key-value pairs if the operation is successful.  If the
		/// file is not found, invalid, or empty, the response will indicate failure or include a warning.</returns>
		public async Task<Response<Dictionary<string, string>>> GetDictionaryAsync(string code)
		{
			if(code == null || code.Length != 2)
			{
				return Response<Dictionary<string, string>>.Fail("Invalid code");
			}
			var filePath = Path.Combine(_localesPath, code.ToLowerInvariant() + ".json");
			if(!File.Exists(filePath))
			{
				return Response<Dictionary<string, string>>.Fail("Dictionary file not found");
			}
			try
			{
				var fileContext = await File.ReadAllTextAsync(filePath);
				var data = new Dictionary<string, string>();
				try
				{
					data = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonDocument.Parse(fileContext));
					if(data == null)
					{
						return Response<Dictionary<string, string>>.SuccessWithWarning(data!, "No data in the file");
					}
					if(data?.Count == 0)
					{
						return Response<Dictionary<string, string>>.SuccessWithWarning(data!, "The list is empty");
					}
					return Response<Dictionary<string, string>>.Successful(data!, code);
				}
				catch(Exception ex)
				{
					return Response<Dictionary<string, string>>.Fail(ex);
				}
			}
			catch(Exception ex)
			{
				return Response<Dictionary<string, string>>.Fail(ex);
			}
		}

		/// <summary>
		/// Asynchronously retrieves the last stored translation data from a file.
		/// </summary>
		/// <remarks>This method attempts to read a JSON file containing translation data and deserialize it into a
		/// dictionary. If the file does not exist, is empty, or an error occurs during reading or deserialization, the method
		/// returns a failure response with an appropriate error message or exception.</remarks>
		/// <returns>A <see cref="Response{T}"/> object containing a dictionary of translation key-value pairs if successful. If the
		/// operation fails, the response will indicate the failure reason.</returns>
		public async Task<Response<Dictionary<string, string>>> GetLastStored()
		{
			if(!File.Exists(_oldTranslationPath))
			{
				return Response<Dictionary<string, string>>.Fail("Not found");
			}
			try
			{
				string oldTranslationJson = await File.ReadAllTextAsync(_oldTranslationPath);
				if(oldTranslationJson == null)
					return Response<Dictionary<string, string>>.Fail("old Translation File is empty");
				if(oldTranslationJson.Length == 0)
					return Response<Dictionary<string, string>>.Fail("old Translation File is empty");
				return new Response<Dictionary<string, string>>() { Data = JsonSerializer.Deserialize<Dictionary<string, string>>(oldTranslationJson) };
			}
			catch(Exception ex)
			{
				return Response<Dictionary<string, string>>.Fail(ex);
			}
		}

		/// <summary>
		/// Asynchronously retrieves all translation dictionaries for the required languages.
		/// </summary>
		/// <remarks>This method identifies the languages for which translations are needed and retrieves the
		/// corresponding dictionaries. The dictionaries are combined into a <see cref="TranslationTree"/> object, which
		/// organizes the translations by language. If no languages are identified, the method returns a failure
		/// response.</remarks>
		/// <returns>A <see cref="Response{T}"/> containing a <see cref="TranslationTree"/> object with the retrieved translation
		/// dictionaries. If no languages are identified, the response indicates failure with an appropriate error message.</returns>
		public async Task<Response<TranslationTree>> GetAllDictionariesAsync()
		{
			var languagesNeeded = TranslationsPresented();
			if(languagesNeeded != null)
			{
				var final = new TranslationTree();
				foreach(var language in languagesNeeded)
				{
					Response<Dictionary<string, string>> response = await GetDictionaryAsync(language);
					final.Translations[language] = response.Data ?? [];
				}
				return new Response<TranslationTree>() { Success = true, Data = final };
			}
			return Response<TranslationTree>.Fail("No files in the folder");
		}

		/// <summary>
		/// Saves a dictionary of key-value pairs to a JSON file associated with the specified language code.
		/// </summary>
		/// <remarks>The method serializes the provided dictionary to JSON format and writes it to a file named after
		/// the language code in the specified locales directory. If the file already exists, it will be
		/// overwritten.</remarks>
		/// <param name="code">A two-character language code representing the target locale. Must not be <see langword="null"/> and must have a
		/// length of 2.</param>
		/// <param name="data">The dictionary of key-value pairs to save. If <see langword="null"/>, an empty dictionary will be saved.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a <see langword="bool"/> value indicating whether the operation was
		/// successful. If successful, the response includes a success message; otherwise, it includes an error message.</returns>
		public async Task<Response<bool>> SaveDictionaryAsync(string code, Dictionary<string, string> data)
		{
			try
			{
				if(code == null)
				{
					return Response<bool>.Fail("Code can't be null");
				}
				if(code.Length != 2)
				{
					return Response<bool>.Fail("Invalid language code");
				}
				var toStore = data == null ? [] : data;
				string json = JsonSerializer.Serialize(toStore);
				var path = Path.Combine(_localesPath, code + ".json");
				await (File.WriteAllTextAsync(path, json));
				return Response<bool>.Successful(true, "Successfully stored");
			}
			catch(Exception ex)
			{
				return Response<bool>.Fail($"Error: {ex.Message}");
			}
		}

		/// <summary>
		/// Saves the provided translation data to a file in JSON format.
		/// </summary>
		/// <remarks>The method serializes the provided dictionary into JSON and writes it to a predefined file path.
		/// If an exception occurs during the operation, the method returns a failure response with the error
		/// message.</remarks>
		/// <param name="data">A dictionary containing translation key-value pairs to be saved. If <paramref name="data"/> is null, an empty
		/// dictionary will be saved.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a boolean value indicating success or failure. Returns <see
		/// langword="true"/> if the data is successfully saved; otherwise, <see langword="false"/>.</returns>
		public async Task<Response<bool>> SaveOldTranslationAsync(Dictionary<string, string> data)
		{
			try
			{
				var toStore = data == null ? [] : data;
				string json = JsonSerializer.Serialize(toStore);
				var path = _oldTranslationPath;
				await (File.WriteAllTextAsync(path, json));
				return Response<bool>.Successful(true, "old translations successfully stored");
			}
			catch(Exception ex)
			{
				return Response<bool>.Fail($"Error: {ex.Message}");
			}
		}

		/// <summary>
		/// Saves the translations from the specified translation tree asynchronously.
		/// </summary>
		/// <remarks>This method processes each language in the translation tree and attempts to save its
		/// corresponding dictionary of translations. The result indicates the success or failure of the save operation for
		/// each language.</remarks>
		/// <param name="translationTree">The <see cref="TranslationTree"/> containing the translations to be saved, organized by language.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{T}"/> where the
		/// data is a dictionary mapping language codes to a boolean value indicating whether the save operation was
		/// successful for each language.</returns>
		public async Task<Response<Dictionary<string, bool>>> SaveTranslationsAsync(TranslationTree translationTree)
		{
			var data = translationTree.Translations;
			var response = new Response<Dictionary<string, bool>>
			{
				Data = []
			};
			foreach(var entry in data)
			{
				string language = entry.Key;
				Dictionary<string, string> dictionary = entry.Value;
				var result = await SaveDictionaryAsync(language, dictionary);
				response.Data[language] = result.Success;
			}
			return response;
		}

		/// <summary>
		/// Retrieves the list of available translations based on the locale files present in the specified directory.
		/// </summary>
		/// <remarks>This method scans the directory specified by the internal locale path for files matching the
		/// pattern "??.json", where "??" represents a two-character language code. The language codes are extracted from the
		/// file names and returned in lowercase.</remarks>
		/// <returns>An array of strings representing the language codes of the available translations.  Each code is derived from the
		/// file names of the locale files and is in lowercase.</returns>
		public string[] TranslationsPresented()
		{
			List<string> result = [];
			var languageFiles = Directory.GetFiles(_localesPath, "??.json");
			foreach(var languageFile in languageFiles)
			{
				result.Add(Path.GetFileNameWithoutExtension(languageFile).ToLowerInvariant());
			}
			return [.. result];
		}

		private readonly List<Language> _languages =
		[
			new Language { Code = "aa", Name = "Afar", Native = "Afar" },
				new Language { Code = "ab", Name = "Abkhazian", Native = "Аҧсуа" },
				new Language { Code = "af", Name = "Afrikaans", Native = "Afrikaans" },
				new Language { Code = "ak", Name = "Akan", Native = "Akana" },
				new Language { Code = "am", Name = "Amharic", Native = "አማርኛ" },
				new Language { Code = "an", Name = "Aragonese", Native = "Aragonés" },
				new Language { Code = "ar", Name = "Arabic", Native = "العربية", Rtl = true },
				new Language { Code = "as", Name = "Assamese", Native = "অসমীয়া" },
				new Language { Code = "av", Name = "Avar", Native = "Авар" },
				new Language { Code = "ay", Name = "Aymara", Native = "Aymar" },
				new Language { Code = "az", Name = "Azerbaijani", Native = "Azərbaycanca" },
				new Language { Code = "ba", Name = "Bashkir", Native = "Башҡорт" },
				new Language { Code = "be", Name = "Belarusian", Native = "Беларуская" },
				new Language { Code = "bg", Name = "Bulgarian", Native = "Български" },
				new Language { Code = "bh", Name = "Bihari", Native = "भोजपुरी" },
				new Language { Code = "bi", Name = "Bislama", Native = "Bislama" },
				new Language { Code = "bm", Name = "Bambara", Native = "Bamanankan" },
				new Language { Code = "bn", Name = "Bengali", Native = "বাংলা" },
				new Language { Code = "bo", Name = "Tibetan", Native = "བོད་ཡིག / Bod skad" },
				new Language { Code = "br", Name = "Breton", Native = "Brezhoneg" },
				new Language { Code = "bs", Name = "Bosnian", Native = "Bosanski" },
				new Language { Code = "ca", Name = "Catalan", Native = "Català" },
				new Language { Code = "ce", Name = "Chechen", Native = "Нохчийн" },
				new Language { Code = "ch", Name = "Chamorro", Native = "Chamoru" },
				new Language { Code = "co", Name = "Corsican", Native = "Corsu" },
				new Language { Code = "cr", Name = "Cree", Native = "Nehiyaw" },
				new Language { Code = "cs", Name = "Czech", Native = "Česky" },
				new Language { Code = "cu", Name = "Old Church Slavonic / Old Bulgarian", Native = "словѣньскъ / slověnĭskŭ" },
				new Language { Code = "cv", Name = "Chuvash", Native = "Чăваш" },
				new Language { Code = "cy", Name = "Welsh", Native = "Cymraeg" },
				new Language { Code = "da", Name = "Danish", Native = "Dansk" },
				new Language { Code = "de", Name = "German", Native = "Deutsch" },
				new Language { Code = "dv", Name = "Divehi", Native = "ދިވެހިބަސް", Rtl = true },
				new Language { Code = "dz", Name = "Dzongkha", Native = "ཇོང་ཁ" },
				new Language { Code = "ee", Name = "Ewe", Native = "Ɛʋɛ" },
				new Language { Code = "el", Name = "Greek", Native = "Ελληνικά" },
				new Language { Code = "en", Name = "English", Native = "English" },
				new Language { Code = "eo", Name = "Esperanto", Native = "Esperanto" },
				new Language { Code = "es", Name = "Spanish", Native = "Español" },
				new Language { Code = "et", Name = "Estonian", Native = "Eesti" },
				new Language { Code = "eu", Name = "Basque", Native = "Euskara" },
				new Language { Code = "fa", Name = "Persian", Native = "فارسی", Rtl = true },
				new Language { Code = "ff", Name = "Peul", Native = "Fulfulde" },
				new Language { Code = "fi", Name = "Finnish", Native = "Suomi" },
				new Language { Code = "fj", Name = "Fijian", Native = "Na Vosa Vakaviti" },
				new Language { Code = "fo", Name = "Faroese", Native = "Føroyskt" },
				new Language { Code = "fr", Name = "French", Native = "Français" },
				new Language { Code = "fy", Name = "West Frisian", Native = "Frysk" },
				new Language { Code = "ga", Name = "Irish", Native = "Gaeilge" },
				new Language { Code = "gd", Name = "Scottish Gaelic", Native = "Gàidhlig" },
				new Language { Code = "gl", Name = "Galician", Native = "Galego" },
				new Language { Code = "gn", Name = "Guarani", Native = "Avañe'ẽ" },
				new Language { Code = "gu", Name = "Gujarati", Native = "ગુજરાતી" },
				new Language { Code = "gv", Name = "Manx", Native = "Gaelg" },
				new Language { Code = "ha", Name = "Hausa", Native = "هَوُسَ", Rtl = true },
				new Language { Code = "he", Name = "Hebrew", Native = "עברית", Rtl = true },
				new Language { Code = "hi", Name = "Hindi", Native = "हिन्दी" },
				new Language { Code = "ho", Name = "Hiri Motu", Native = "Hiri Motu" },
				new Language { Code = "hr", Name = "Croatian", Native = "Hrvatski" },
				new Language { Code = "ht", Name = "Haitian", Native = "Krèyol ayisyen" },
				new Language { Code = "hu", Name = "Hungarian", Native = "Magyar" },
				new Language { Code = "hy", Name = "Armenian", Native = "Հայերեն" },
				new Language { Code = "hz", Name = "Herero", Native = "Otsiherero" },
				new Language { Code = "ia", Name = "Interlingua", Native = "Interlingua" },
				new Language { Code = "id", Name = "Indonesian", Native = "Bahasa Indonesia" },
				new Language { Code = "ie", Name = "Interlingue", Native = "Interlingue" },
				new Language { Code = "ig", Name = "Igbo", Native = "Igbo" },
				new Language { Code = "ii", Name = "Sichuan Yi", Native = "ꆇꉙ / 四川彝语" },
				new Language { Code = "ik", Name = "Inupiak", Native = "Iñupiak" },
				new Language { Code = "io", Name = "Ido", Native = "Ido" },
				new Language { Code = "is", Name = "Icelandic", Native = "Íslenska" },
				new Language { Code = "it", Name = "Italian", Native = "Italiano" },
				new Language { Code = "iu", Name = "Inuktitut", Native = "ᐃᓄᒃᑎᑐᑦ" },
				new Language { Code = "ja", Name = "Japanese", Native = "日本語" },
				new Language { Code = "jv", Name = "Javanese", Native = "Basa Jawa" },
				new Language { Code = "ka", Name = "Georgian", Native = "ქართული" },
				new Language { Code = "kg", Name = "Kongo", Native = "KiKongo" },
				new Language { Code = "ki", Name = "Kikuyu", Native = "Gĩkũyũ" },
				new Language { Code = "kj", Name = "Kuanyama", Native = "Kuanyama" },
				new Language { Code = "kk", Name = "Kazakh", Native = "Қазақша" },
				new Language { Code = "kl", Name = "Greenlandic", Native = "Kalaallisut" },
				new Language { Code = "km", Name = "Cambodian", Native = "ភាសាខ្មែរ" },
				new Language { Code = "kn", Name = "Kannada", Native = "ಕನ್ನಡ" },
				new Language { Code = "ko", Name = "Korean", Native = "한국어" },
				new Language { Code = "kr", Name = "Kanuri", Native = "Kanuri" },
				new Language { Code = "ks", Name = "Kashmiri", Native = "कश्मीरी / كشميري", Rtl = true },
				new Language { Code = "ku", Name = "Kurdish", Native = "Kurdî / كوردی", Rtl = true },
				new Language { Code = "kv", Name = "Komi", Native = "Коми" },
				new Language { Code = "kw", Name = "Cornish", Native = "Kernewek" },
				new Language { Code = "ky", Name = "Kirghiz", Native = "Kırgızca / Кыргызча" },
				new Language { Code = "la", Name = "Latin", Native = "Latina" },
				new Language { Code = "lb", Name = "Luxembourgish", Native = "Lëtzebuergesch" },
				new Language { Code = "lg", Name = "Ganda", Native = "Luganda" },
				new Language { Code = "li", Name = "Limburgian", Native = "Limburgs" },
				new Language { Code = "ln", Name = "Lingala", Native = "Lingála" },
				new Language { Code = "lo", Name = "Laotian", Native = "ລາວ / Pha xa lao" },
				new Language { Code = "lt", Name = "Lithuanian", Native = "Lietuvių" },
				new Language { Code = "lu", Name = "Luba-Katanga", Native = "Tshiluba" },
				new Language { Code = "lv", Name = "Latvian", Native = "Latviešu" },
				new Language { Code = "mg", Name = "Malagasy", Native = "Malagasy" },
				new Language { Code = "mh", Name = "Marshallese", Native = "Kajin Majel / Ebon" },
				new Language { Code = "mi", Name = "Maori", Native = "Māori" },
				new Language { Code = "mk", Name = "Macedonian", Native = "Македонски" },
				new Language { Code = "ml", Name = "Malayalam", Native = "മലയാളം" },
				new Language { Code = "mn", Name = "Mongolian", Native = "Монгол" },
				new Language { Code = "mo", Name = "Moldovan", Native = "Moldovenească" },
				new Language { Code = "mr", Name = "Marathi", Native = "मराठी" },
				new Language { Code = "ms", Name = "Malay", Native = "Bahasa Melayu" },
				new Language { Code = "mt", Name = "Maltese", Native = "bil-Malti" },
				new Language { Code = "my", Name = "Burmese", Native = "မြန်မာစာ" },
				new Language { Code = "na", Name = "Nauruan", Native = "Dorerin Naoero" },
				new Language { Code = "nb", Name = "Norwegian Bokmål", Native = "Norsk bokmål" },
				new Language { Code = "nd", Name = "North Ndebele", Native = "Sindebele" },
				new Language { Code = "ne", Name = "Nepali", Native = "नेपाली" },
				new Language { Code = "ng", Name = "Ndonga", Native = "Oshiwambo" },
				new Language { Code = "nl", Name = "Dutch", Native = "Nederlands" },
				new Language { Code = "nn", Name = "Norwegian Nynorsk", Native = "Norsk nynorsk" },
				new Language { Code = "no", Name = "Norwegian", Native = "Norsk" },
				new Language { Code = "nr", Name = "South Ndebele", Native = "isiNdebele" },
				new Language { Code = "nv", Name = "Navajo", Native = "Diné bizaad" },
				new Language { Code = "ny", Name = "Chichewa", Native = "Chi-Chewa" },
				new Language { Code = "oc", Name = "Occitan", Native = "Occitan" },
				new Language { Code = "oj", Name = "Ojibwa", Native = "ᐊᓂᔑᓈᐯᒧᐎᓐ / Anishinaabemowin" },
				new Language { Code = "om", Name = "Oromo", Native = "Oromoo" },
				new Language { Code = "or", Name = "Oriya", Native = "ଓଡ଼ିଆ" },
				new Language { Code = "os", Name = "Ossetian / Ossetic", Native = "Иронау" },
				new Language { Code = "pa", Name = "Panjabi / Punjabi", Native = "ਪੰਜਾਬੀ / पंजाबी / پنجابي" },
				new Language { Code = "pi", Name = "Pali", Native = "Pāli / पाऴि" },
				new Language { Code = "pl", Name = "Polish", Native = "Polski" },
				new Language { Code = "ps", Name = "Pashto", Native = "پښتو", Rtl = true },
				new Language { Code = "pt", Name = "Portuguese", Native = "Português" },
				new Language { Code = "qu", Name = "Quechua", Native = "Runa Simi" },
				new Language { Code = "rm", Name = "Raeto Romance", Native = "Rumantsch" },
				new Language { Code = "rn", Name = "Kirundi", Native = "Kirundi" },
				new Language { Code = "ro", Name = "Romanian", Native = "Română" },
				new Language { Code = "ru", Name = "Russian", Native = "Русский" },
				new Language { Code = "rw", Name = "Rwandi", Native = "Kinyarwandi" },
				new Language { Code = "sa", Name = "Sanskrit", Native = "संस्कृतम्" },
				new Language { Code = "sc", Name = "Sardinian", Native = "Sardu" },
				new Language { Code = "sd", Name = "Sindhi", Native = "सिनधि" },
				new Language { Code = "se", Name = "Northern Sami", Native = "Sámegiella" },
				new Language { Code = "sg", Name = "Sango", Native = "Sängö" },
				new Language { Code = "sh", Name = "Serbo-Croatian", Native = "Srpskohrvatski / Српскохрватски" },
				new Language { Code = "si", Name = "Sinhalese", Native = "සිංහල" },
				new Language { Code = "sk", Name = "Slovak", Native = "Slovenčina" },
				new Language { Code = "sl", Name = "Slovenian", Native = "Slovenščina" },
				new Language { Code = "sm", Name = "Samoan", Native = "Gagana Samoa" },
				new Language { Code = "sn", Name = "Shona", Native = "chiShona" },
				new Language { Code = "so", Name = "Somalia", Native = "Soomaaliga" },
				new Language { Code = "sq", Name = "Albanian", Native = "Shqip" },
				new Language { Code = "sr", Name = "Serbian", Native = "Српски" },
				new Language { Code = "ss", Name = "Swati", Native = "SiSwati" },
				new Language { Code = "st", Name = "Southern Sotho", Native = "Sesotho" },
				new Language { Code = "su", Name = "Sundanese", Native = "Basa Sunda" },
				new Language { Code = "sv", Name = "Swedish", Native = "Svenska" },
				new Language { Code = "sw", Name = "Swahili", Native = "Kiswahili" },
				new Language { Code = "ta", Name = "Tamil", Native = "தமிழ்" },
				new Language { Code = "te", Name = "Telugu", Native = "తెలుగు" },
				new Language { Code = "tg", Name = "Tajik", Native = "Тоҷикӣ" },
				new Language { Code = "th", Name = "Thai", Native = "ไทย / Phasa Thai" },
				new Language { Code = "ti", Name = "Tigrinya", Native = "ትግርኛ" },
				new Language { Code = "tk", Name = "Turkmen", Native = "Туркмен / تركمن" },
				new Language { Code = "tl", Name = "Tagalog / Filipino", Native = "Tagalog" },
				new Language { Code = "tn", Name = "Tswana", Native = "Setswana" },
				new Language { Code = "to", Name = "Tonga", Native = "Lea Faka-Tonga" },
				new Language { Code = "tr", Name = "Turkish", Native = "Türkçe" },
				new Language { Code = "ts", Name = "Tsonga", Native = "Xitsonga" },
				new Language { Code = "tt", Name = "Tatar", Native = "Tatarça" },
				new Language { Code = "tw", Name = "Twi", Native = "Twi" },
				new Language { Code = "ty", Name = "Tahitian", Native = "Reo Mā`ohi" },
				new Language { Code = "ug", Name = "Uyghur", Native = "Uyƣurqə / ئۇيغۇرچە" },
				new Language { Code = "uk", Name = "Ukrainian", Native = "Українська" },
				new Language { Code = "ur", Name = "Urdu", Native = "اردو", Rtl = true },
				new Language { Code = "uz", Name = "Uzbek", Native = "Ўзбек" },
				new Language { Code = "ve", Name = "Venda", Native = "Tshivenḓa" },
				new Language { Code = "vi", Name = "Vietnamese", Native = "Tiếng Việt" },
				new Language { Code = "vo", Name = "Volapük", Native = "Volapük" },
				new Language { Code = "wa", Name = "Walloon", Native = "Walon" },
				new Language { Code = "wo", Name = "Wolof", Native = "Wollof" },
				new Language { Code = "xh", Name = "Xhosa", Native = "isiXhosa" },
				new Language { Code = "yi", Name = "Yiddish", Native = "ייִדיש", Rtl = true },
				new Language { Code = "yo", Name = "Yoruba", Native = "Yorùbá" },
				new Language { Code = "za", Name = "Zhuang", Native = "Cuengh / Tôô / 壮语" },
				new Language { Code = "zh", Name = "Chinese", Native = "中文" },
				new Language { Code = "zu", Name = "Zulu", Native = "isiZulu" }
		  ];
	}
}