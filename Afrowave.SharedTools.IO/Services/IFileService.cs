using Afrowave.SharedTools.IO.Models;
using Afrowave.SharedTools.Models.Results;
using System.Text;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.IO.Services
{
   /// <summary>
   /// Provides high-level file IO operations for text, binary, and object serialization.
   /// </summary>
   /// <remarks>
   /// Supports JSON, XML, CSV, and YAML formats. Defaults use UTF-8 without BOM, JSON/YAML camelCase,
   /// XML indented output, and CSV with header and invariant culture. The <c>TOptions</c> parameter can be a native
   /// options type to override defaults (e.g., <see cref="System.Text.Json.JsonSerializerOptions"/>,
   /// <see cref="CsvHelper.Configuration.CsvConfiguration"/>, <see cref="System.Xml.XmlWriterSettings"/>,

   /// </remarks>
   public interface IFileService
   {
      /// <summary>
      /// Reads the entire file as a byte array.
      /// </summary>
      /// <param name="filePath">The path to the file.</param>
      /// <returns>A response containing the file content.</returns>
      Response<byte[]> ReadBytes(string filePath);

      /// <summary>
      /// Asynchronously reads the entire file as a byte array.
      /// </summary>
      /// <param name="filePath">The path to the file.</param>
      /// <returns>A response containing the file content.</returns>
      Task<Response<byte[]>> ReadBytesAsync(string filePath);

      /// <summary>
      /// Reads and deserializes a CSV file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The data type to deserialize to (single record or a collection type).</typeparam>
      /// <typeparam name="TOptions">CSV options type, e.g., <see cref="CsvHelper.Configuration.CsvConfiguration"/>.</typeparam>
      /// <param name="filePath">The path to the CSV file.</param>
      /// <param name="options">Optional CSV options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Response<TData> ReadObjectFromCsvFile<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Asynchronously reads and deserializes a CSV file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The data type to deserialize to (single record or a collection type).</typeparam>
      /// <typeparam name="TOptions">CSV options type, e.g., <see cref="CsvHelper.Configuration.CsvConfiguration"/>.</typeparam>
      /// <param name="filePath">The path to the CSV file.</param>
      /// <param name="options">Optional CSV options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Task<Response<TData>> ReadObjectFromCsvFileAsync<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Reads and deserializes an object from a file based on the specified file type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">Format-specific options type.</typeparam>
      /// <param name="filePath">The path to the file.</param>
      /// <param name="fileType">The file format.</param>
      /// <param name="options">Optional format-specific options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Response<TData> ReadObjectFromFile<TData, TOptions>(string filePath, FileType fileType, TOptions options);

      /// <summary>
      /// Asynchronously reads and deserializes an object from a file based on the specified file type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">Format-specific options type.</typeparam>
      /// <param name="filePath">The path to the file.</param>
      /// <param name="fileType">The file format.</param>
      /// <param name="options">Optional format-specific options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Task<Response<TData>> ReadObjectFromFileAsync<TData, TOptions>(string filePath, FileType fileType, TOptions options);

      /// <summary>
      /// Reads and deserializes a JSON file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">JSON options type, e.g., <see cref="System.Text.Json.JsonSerializerOptions"/>.</typeparam>
      /// <param name="filePath">The path to the JSON file.</param>
      /// <param name="options">Optional JSON options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Response<TData> ReadObjectFromJsonFile<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Asynchronously reads and deserializes a JSON file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">JSON options type, e.g., <see cref="System.Text.Json.JsonSerializerOptions"/>.</typeparam>
      /// <param name="filePath">The path to the JSON file.</param>
      /// <param name="options">Optional JSON options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Task<Response<TData>> ReadObjectFromJsonFileAsync<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Reads and deserializes an XML file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">XML options type, e.g., <see cref="System.Xml.XmlReaderSettings"/>.</typeparam>
      /// <param name="filePath">The path to the XML file.</param>
      /// <param name="options">Optional XML options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Response<TData> ReadObjectFromXmlFile<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Asynchronously reads and deserializes an XML file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">XML options type, e.g., <see cref="System.Xml.XmlReaderSettings"/>.</typeparam>
      /// <param name="filePath">The path to the XML file.</param>
      /// <param name="options">Optional XML options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Task<Response<TData>> ReadObjectFromXmlFileAsync<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Reads and deserializes a YAML file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">YAML options type, e.g., <see cref="YamlDotNet.Serialization.IDeserializer"/>.</typeparam>
      /// <param name="filePath">The path to the YAML file.</param>
      /// <param name="options">Optional YAML options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Response<TData> ReadObjectFromYamlFile<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Asynchronously reads and deserializes a YAML file to the specified type.
      /// </summary>
      /// <typeparam name="TData">The target data type.</typeparam>
      /// <typeparam name="TOptions">YAML options type, e.g., <see cref="YamlDotNet.Serialization.IDeserializer"/>.</typeparam>
      /// <param name="filePath">The path to the YAML file.</param>
      /// <param name="options">Optional YAML options; defaults are used when null or not provided.</param>
      /// <returns>A response containing the deserialized object.</returns>
      Task<Response<TData>> ReadObjectFromYamlFileAsync<TData, TOptions>(string filePath, TOptions options);

      /// <summary>
      /// Reads the entire file as text.
      /// </summary>
      /// <param name="filePath">The path to the file.</param>
      /// <param name="encoding">Optional encoding; defaults to UTF-8 without BOM.</param>
      /// <returns>A response containing the file content.</returns>
      Response<string> ReadText(string filePath, Encoding? encoding = null);

      /// <summary>
      /// Asynchronously reads the entire file as text.
      /// </summary>
      /// <param name="filePath">The path to the file.</param>
      /// <param name="encoding">Optional encoding; defaults to UTF-8 without BOM.</param>
      /// <returns>A response containing the file content.</returns>
      Task<Response<string>> ReadTextAsync(string filePath, Encoding? encoding = null);

      /// <summary>
      /// Serializes and writes a CSV representation of the specified object to a file.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">CSV options type, e.g., <see cref="CsvHelper.Configuration.CsvConfiguration"/>.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="options">Optional CSV options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Result StoreObjectToCsvFile<TData, TOptions>(TData obj, string filePath, TOptions options);

      /// <summary>
      /// Asynchronously serializes and writes a CSV representation of the specified object to a file.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">CSV options type, e.g., <see cref="CsvHelper.Configuration.CsvConfiguration"/>.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="options">Optional CSV options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Task<Result> StoreObjectToCsvFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options);

      /// <summary>
      /// Serializes and writes an object to a file using the specified format.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">Format-specific options type.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="fileType">The file format to write.</param>
      /// <param name="options">Optional format-specific options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Result StoreObjectToFile<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options);

      /// <summary>
      /// Asynchronously serializes and writes an object to a file using the specified format.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">Format-specific options type.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="fileType">The file format to write.</param>
      /// <param name="options">Optional format-specific options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Task<Result> StoreObjectToFileAsync<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options);

      /// <summary>
      /// Serializes and writes a JSON representation of the specified object to a file.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">JSON options type, e.g., <see cref="System.Text.Json.JsonSerializerOptions"/>.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="options">Optional JSON options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Result StoreObjectToJsonFile<TData, TOptions>(TData obj, string filePath, TOptions options);

      /// <summary>
      /// Asynchronously serializes and writes a JSON representation of the specified object to a file.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">JSON options type, e.g., <see cref="System.Text.Json.JsonSerializerOptions"/>.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="options">Optional JSON options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Task<Result> StoreObjectToJsonFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options);

      /// <summary>
      /// Serializes and writes an XML representation of the specified object to a file.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">XML options type, e.g., <see cref="System.Xml.XmlWriterSettings"/>.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="options">Optional XML options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Result StoreObjectToXmlFile<TData, TOptions>(TData obj, string filePath, TOptions options);

      /// <summary>
      /// Asynchronously serializes and writes an XML representation of the specified object to a file.
      /// </summary>
      /// <typeparam name="TData">The data type to serialize.</typeparam>
      /// <typeparam name="TOptions">XML options type, e.g., <see cref="System.Xml.XmlWriterSettings"/>.</typeparam>
      /// <param name="obj">The object to serialize.</param>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="options">Optional XML options; defaults are used when null or not provided.</param>
      /// <returns>A result indicating success or failure.</returns>
      Task<Result> StoreObjectToXmlFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options);

      /// <summary>
      /// Writes the specified bytes to a file.
      /// </summary>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="data">The bytes to write.</param>
      /// <returns>A result indicating success or failure.</returns>
      Result WriteBytes(string filePath, byte[] data);

      /// <summary>
      /// Asynchronously writes the specified bytes to a file.
      /// </summary>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="data">The bytes to write.</param>
      /// <returns>A result indicating success or failure.</returns>
      Task<Result> WriteBytesAsync(string filePath, byte[] data);

      /// <summary>
      /// Writes the specified text to a file using the given encoding.
      /// </summary>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="content">The text content to write.</param>
      /// <param name="encoding">Optional text encoding; defaults to UTF-8 without BOM.</param>
      /// <returns>A result indicating success or failure.</returns>
      Result WriteText(string filePath, string content, Encoding? encoding = null);

      /// <summary>
      /// Asynchronously writes the specified text to a file using the given encoding.
      /// </summary>
      /// <param name="filePath">The destination file path.</param>
      /// <param name="content">The text content to write.</param>
      /// <param name="encoding">Optional text encoding; defaults to UTF-8 without BOM.</param>
      /// <returns>A result indicating success or failure.</returns>
      Task<Result> WriteTextAsync(string filePath, string content, Encoding? encoding = null);
   }
}