using Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.Plugin.Backend.JsonBackend.Services
{
	/// <summary>
	/// Parses JSON content and determines its structure.
	/// Can return either flat dictionary or structured tree.
	/// </summary>
	public sealed class JsonReaderService
	{
		private readonly ILogger<JsonReaderService> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonReaderService"/> class.
		/// </summary>
		/// <param name="logger">The logger instance used to record diagnostic and operational messages.</param>
		public JsonReaderService(ILogger<JsonReaderService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Parses the provided JSON string and determines its structure.
		/// </summary>
		/// <remarks>This method attempts to parse the JSON string and classify its structure as either flat or
		/// hierarchical. If the JSON is invalid or cannot be parsed, an invalid result is returned with an error
		/// message.</remarks>
		/// <param name="json">The JSON string to parse. Must be a valid JSON document.</param>
		/// <returns>A <see cref="ReadJsonResult"/> representing the parsed JSON.  If the JSON is recognized as a flat object, the
		/// result contains a dictionary of key-value pairs. If the JSON is recognized as a structured tree, the result
		/// contains a hierarchical representation. Returns an invalid result if the JSON is malformed or the root element is
		/// not an object.</returns>
		public async Task<ReadJsonResult> ParseAsync(string json)
		{
			await Task.Yield(); // ensure true async boundary

			try
			{
				using var doc = JsonDocument.Parse(json);
				var root = doc.RootElement;

				if(root.ValueKind != JsonValueKind.Object)
				{
					_logger.LogWarning("Invalid JSON: root element is not an object.");
					return ReadJsonResult.Invalid("Root element is not an object.");
				}

				if(IsFlatObject(root))
				{
					_logger.LogInformation("JSON recognized as flat structure.");
					var flatDict = new Dictionary<string, string>();

					foreach(var prop in root.EnumerateObject())
					{
						flatDict[prop.Name] = prop.Value.GetString() ?? string.Empty;
					}

					return ReadJsonResult.FromFlat(flatDict);
				}

				_logger.LogInformation("JSON recognized as structured tree.");
				var tree = ParseTree(root);
				return ReadJsonResult.FromStructured(tree);
			}
			catch(JsonException ex)
			{
				_logger.LogError(ex, "Failed to parse JSON.");
				return ReadJsonResult.Invalid($"JSON parsing error: {ex.Message}");
			}
		}

		private bool IsFlatObject(JsonElement root)
		{
			foreach(var prop in root.EnumerateObject())
			{
				if(prop.Value.ValueKind != JsonValueKind.String)
					return false;
			}

			return true;
		}

		private TranslationTree ParseTree(JsonElement element)
		{
			var node = new TranslationTree();

			foreach(var prop in element.EnumerateObject())
			{
				if(prop.Value.ValueKind == JsonValueKind.Object)
				{
					node.Children[prop.Name] = ParseTree(prop.Value);
				}
				else if(prop.Value.ValueKind == JsonValueKind.String)
				{
					node.Children[prop.Name] = new TranslationTree
					{
						Value = prop.Value.GetString()
					};
				}
			}

			return node;
		}
	}
}