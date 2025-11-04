using System.Text;
using System.Threading.Tasks;
using Afrowave.SharedTools.IO.Models;
using Afrowave.SharedTools.IO.Services;
using Afrowave.SharedTools.Models.Results;

namespace Afrowave.SharedTools.IO.Static
{
   /// <summary>
   /// Static convenience API mirroring <see cref="IFileService"/> for simple, ad-hoc file IO.
   /// </summary>
   /// <remarks>
   /// Internally uses a single <see cref="FileService"/> instance. For DI-enabled apps prefer injecting
   /// <see cref="IFileService"/>.
   /// </remarks>
   public static class FileHelper
   {
      private static readonly IFileService _svc = new FileService();

      // --------- Async ---------

      // Store

      /// <summary>
      /// Asynchronously serializes the specified object to JSON and stores it in a file at the given path, using the
      /// provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to serialize and store.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the serialization or file storage behavior.</typeparam>
      /// <param name="obj">The object to serialize and store in the JSON file. Cannot be null.</param>
      /// <param name="filePath">The path to the file where the JSON data will be written. Must be a valid file path and cannot be null or
      /// empty.</param>
      /// <param name="options">An options object that configures serialization or file storage behavior. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous operation. The result contains information about the success or
      /// failure of the storage operation.</returns>
      public static Task<Result> StoreObjectToJsonFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
         => _svc.StoreObjectToJsonFileAsync(obj, filePath, options);

      /// <summary>
      /// Asynchronously serializes the specified object to XML and stores it in a file at the given path using the
      /// provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to serialize to XML.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the XML serialization and file storage behavior.</typeparam>
      /// <param name="obj">The object to serialize and store. Cannot be null.</param>
      /// <param name="filePath">The path of the file where the XML representation will be saved. Must be a valid file path.</param>
      /// <param name="options">An object containing options that control the serialization and file writing process. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous operation. The result indicates whether the object was successfully
      /// stored.</returns>
      public static Task<Result> StoreObjectToXmlFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
         => _svc.StoreObjectToXmlFileAsync(obj, filePath, options);

      /// <summary>
      /// Asynchronously stores the specified object to a CSV file at the given path using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to be serialized and stored in the CSV file.</typeparam>
      /// <typeparam name="TOptions">The type containing configuration options for CSV serialization and file storage.</typeparam>
      /// <param name="obj">The object to serialize and write to the CSV file. Cannot be null.</param>
      /// <param name="filePath">The path to the CSV file where the object will be stored. Must be a valid file path.</param>
      /// <param name="options">An object containing options that control how the CSV file is generated and stored.</param>
      /// <returns>A task that represents the asynchronous operation. The result indicates whether the object was successfully stored.</returns>
      public static Task<Result> StoreObjectToCsvFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
         => _svc.StoreObjectToCsvFileAsync(obj, filePath, options);

      /// <summary>
      /// Asynchronously stores the specified object to a file using the provided serialization options and file type.
      /// </summary>
      /// <typeparam name="TData">The type of the object to be stored.</typeparam>
      /// <typeparam name="TOptions">The type of the serialization options to use when storing the object.</typeparam>
      /// <param name="obj">The object to store in the file. Cannot be null.</param>
      /// <param name="filePath">The path to the file where the object will be stored. Must be a valid file path.</param>
      /// <param name="fileType">The format in which to store the object, such as JSON or XML.</param>
      /// <param name="options">The serialization options to apply when storing the object.</param>
      /// <returns>A task that represents the asynchronous operation. The result contains information about the success or
      /// failure of the storage operation.</returns>
      public static Task<Result> StoreObjectToFileAsync<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options)
         => _svc.StoreObjectToFileAsync(obj, filePath, fileType, options);

      /// <summary>
      /// Asynchronously writes the specified text content to a file at the given path, using the provided encoding if
      /// specified.
      /// </summary>
      /// <param name="filePath">The path to the file where the text content will be written. Cannot be null or empty.</param>
      /// <param name="content">The text content to write to the file. Cannot be null.</param>
      /// <param name="encoding">The character encoding to use when writing the file. If null, UTF-8 encoding is used by default.</param>
      /// <returns>A task that represents the asynchronous write operation. The result indicates success or failure, including
      /// any error information.</returns>
      public static Task<Result> WriteTextAsync(string filePath, string content, Encoding? encoding = null)
         => _svc.WriteTextAsync(filePath, content, encoding);

      /// <summary>
      /// Asynchronously writes the specified byte array to a file at the given path, overwriting any existing content.
      /// </summary>
      /// <param name="filePath">The path of the file to which the data will be written. Cannot be null or empty.</param>
      /// <param name="data">The byte array containing the data to write to the file. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous write operation. The result indicates whether the operation succeeded
      /// or failed.</returns>
      public static Task<Result> WriteBytesAsync(string filePath, byte[] data)
         => _svc.WriteBytesAsync(filePath, data);

      // Read
      /// <summary>
      /// Asynchronously reads and deserializes an object of the specified type from a file using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to deserialize from the file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the deserialization process.</typeparam>
      /// <param name="filePath">The path to the file containing the serialized object. Cannot be null or empty.</param>
      /// <param name="fileType">The format of the file to read, such as JSON or XML.</param>
      /// <param name="options">An object containing options that control how the file is read and deserialized.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{TData}"/>
      /// with the deserialized object and operation status information.</returns>
      public static Task<Response<TData>> ReadObjectFromFileAsync<TData, TOptions>(string filePath, FileType fileType, TOptions options)
         => _svc.ReadObjectFromFileAsync<TData, TOptions>(filePath, fileType, options);

      /// <summary>
      /// Asynchronously reads a JSON file from the specified path and deserializes its contents into an object of type
      /// <typeparamref name="TData"/> using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of object to deserialize from the JSON file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the deserialization process.</typeparam>
      /// <param name="filePath">The path to the JSON file to read. Cannot be null or empty.</param>
      /// <param name="options">An object containing options that control the deserialization behavior. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{TData}"/>
      /// with the deserialized object if successful; otherwise, an error response.</returns>
      public static Task<Response<TData>> ReadObjectFromJsonFileAsync<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromJsonFileAsync<TData, TOptions>(filePath, options);

      /// <summary>
      /// Asynchronously reads a single object from a CSV file and returns the result wrapped in a response object.
      /// </summary>
      /// <typeparam name="TData">The type of object to deserialize from the CSV file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the CSV reading and deserialization process.</typeparam>
      /// <param name="filePath">The path to the CSV file to read. Cannot be null or empty.</param>
      /// <param name="options">An options object that specifies settings for reading and deserializing the CSV file. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a response object with the
      /// deserialized data if successful; otherwise, an error response.</returns>
      public static Task<Response<TData>> ReadObjectFromCsvFileAsync<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromCsvFileAsync<TData, TOptions>(filePath, options);

      /// <summary>
      /// Asynchronously reads an object of the specified type from an XML file using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to deserialize from the XML file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the XML deserialization process.</typeparam>
      /// <param name="filePath">The path to the XML file to read. Cannot be null or empty.</param>
      /// <param name="options">An object containing options that control how the XML is read and deserialized. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{TData}"/>
      /// with the deserialized object if successful; otherwise, an error response.</returns>
      public static Task<Response<TData>> ReadObjectFromXmlFileAsync<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromXmlFileAsync<TData, TOptions>(filePath, options);

      /// <summary>
      /// Asynchronously reads a YAML file from the specified path and deserializes its contents into an object of type
      /// <typeparamref name="TData"/> using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of object to deserialize from the YAML file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the deserialization process.</typeparam>
      /// <param name="filePath">The path to the YAML file to read. Cannot be null or empty.</param>
      /// <param name="options">An object containing options that control how the YAML file is deserialized. Cannot be null.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{TData}"/>
      /// with the deserialized object and operation status information.</returns>
      public static Task<Response<TData>> ReadObjectFromYamlFileAsync<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromYamlFileAsync<TData, TOptions>(filePath, options);

      /// <summary>
      /// Asynchronously reads all text from the specified file and returns the result as a string.
      /// </summary>
      /// <param name="filePath">The path to the file to read. Cannot be null or empty.</param>
      /// <param name="encoding">The character encoding to use when reading the file. If null, the default encoding is used.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Response{string}"/>
      /// with the file's text content.</returns>
      public static Task<Response<string>> ReadTextAsync(string filePath, Encoding? encoding = null)
         => _svc.ReadTextAsync(filePath, encoding);

      /// <summary>
      /// Asynchronously reads all bytes from the specified file and returns the result in a response object.
      /// </summary>
      /// <param name="filePath">The path to the file to read. Must not be null, empty, or contain invalid path characters.</param>
      /// <returns>A task that represents the asynchronous operation. The task result contains a response object with the file's
      /// byte contents if the read succeeds; otherwise, the response indicates the error.</returns>
      public static Task<Response<byte[]>> ReadBytesAsync(string filePath)
         => _svc.ReadBytesAsync(filePath);

      // --------- Sync ---------

      // Store
      /// <summary>
      /// Serializes the specified object to JSON and stores it in a file at the given path using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to serialize and store.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the serialization or file storage behavior.</typeparam>
      /// <param name="obj">The object to serialize and write to the JSON file. Cannot be null.</param>
      /// <param name="filePath">The path to the file where the JSON data will be stored. Must be a valid file path and cannot be null or
      /// empty.</param>
      /// <param name="options">An options object that configures serialization or file writing behavior. Cannot be null.</param>
      /// <returns>A Result indicating whether the object was successfully stored. Contains error information if the operation
      /// fails.</returns>
      public static Result StoreObjectToJsonFile<TData, TOptions>(TData obj, string filePath, TOptions options)
         => _svc.StoreObjectToJsonFile(obj, filePath, options);

      /// <summary>
      /// Serializes the specified object to XML and stores it in a file at the given path using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to serialize to XML.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the XML serialization and file storage process.</typeparam>
      /// <param name="obj">The object to serialize and store in the XML file. Cannot be null.</param>
      /// <param name="filePath">The path to the file where the XML representation of the object will be saved. Must be a valid file path.</param>
      /// <param name="options">An object containing options that control the XML serialization and file writing behavior. Cannot be null.</param>
      /// <returns>A Result indicating whether the object was successfully serialized and stored. Contains error information if
      /// the operation fails.</returns>
      public static Result StoreObjectToXmlFile<TData, TOptions>(TData obj, string filePath, TOptions options)
         => _svc.StoreObjectToXmlFile(obj, filePath, options);

      /// <summary>
      /// Stores the specified object as a CSV file at the given file path using the provided options.
      /// </summary>
      /// <remarks>If the file already exists, it may be overwritten depending on the options provided. This
      /// method does not guarantee thread safety for concurrent writes to the same file.</remarks>
      /// <typeparam name="TData">The type of the object to be serialized and stored in the CSV file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the CSV serialization and storage process.</typeparam>
      /// <param name="obj">The object to serialize and store in the CSV file. Cannot be null.</param>
      /// <param name="filePath">The path to the file where the CSV data will be written. Must be a valid file path and cannot be null or
      /// empty.</param>
      /// <param name="options">The options that control how the object is serialized to CSV and stored. Cannot be null.</param>
      /// <returns>A Result indicating whether the object was successfully stored. Contains error information if the operation
      /// fails.</returns>
      public static Result StoreObjectToCsvFile<TData, TOptions>(TData obj, string filePath, TOptions options)
         => _svc.StoreObjectToCsvFile(obj, filePath, options);

      /// <summary>
      /// Serializes the specified object and stores it in a file using the provided file type and options.
      /// </summary>
      /// <typeparam name="TData">The type of the object to serialize and store.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the serialization and storage process.</typeparam>
      /// <param name="obj">The object to serialize and store in the file. Cannot be null.</param>
      /// <param name="filePath">The path of the file where the object will be stored. Must be a valid file path and cannot be null or empty.</param>
      /// <param name="fileType">The format in which the object will be stored, such as JSON or XML.</param>
      /// <param name="options">An options object that configures serialization and storage behavior. The specific options required depend on
      /// the file type.</param>
      /// <returns>A Result indicating whether the object was successfully stored. Contains error information if the operation
      /// fails.</returns>
      public static Result StoreObjectToFile<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options)
         => _svc.StoreObjectToFile(obj, filePath, fileType, options);

      /// <summary>
      /// Writes the specified text content to a file at the given path, using the provided encoding if specified.
      /// </summary>
      /// <param name="filePath">The path of the file to which the text content will be written. Cannot be null or empty.</param>
      /// <param name="content">The text content to write to the file. If null, an empty file will be created.</param>
      /// <param name="encoding">The character encoding to use when writing the file. If null, UTF-8 encoding is used by default.</param>
      /// <returns>A Result object indicating the success or failure of the write operation. Contains error information if the
      /// operation fails.</returns>
      public static Result WriteText(string filePath, string content, Encoding? encoding = null)
         => _svc.WriteText(filePath, content, encoding);

      /// <summary>
      /// Writes the specified byte array to a file at the given path, overwriting any existing content.
      /// </summary>
      /// <param name="filePath">The path to the file where the data will be written. Cannot be null or empty.</param>
      /// <param name="data">The byte array containing the data to write to the file. Cannot be null.</param>
      /// <returns>A Result object indicating whether the write operation succeeded or failed.</returns>
      public static Result WriteBytes(string filePath, byte[] data)
         => _svc.WriteBytes(filePath, data);

      // Read
      /// <summary>
      /// Reads an object of the specified type from a file using the provided options and file type.
      /// </summary>
      /// <typeparam name="TData">The type of object to deserialize from the file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to control the deserialization process.</typeparam>
      /// <param name="filePath">The path to the file containing the serialized object. Cannot be null or empty.</param>
      /// <param name="fileType">The format of the file to read, which determines how the object is deserialized.</param>
      /// <param name="options">An options object that configures the deserialization behavior. The specific options required depend on the
      /// file type and deserialization implementation.</param>
      /// <returns>A Response TData containing the deserialized object if successful; otherwise, an error response indicating the failure.</returns>
      public static Response<TData> ReadObjectFromFile<TData, TOptions>(string filePath, FileType fileType, TOptions options)
         => _svc.ReadObjectFromFile<TData, TOptions>(filePath, fileType, options);

      /// <summary>
      /// Reads a JSON file from the specified path and deserializes its contents into an object of the specified type.
      /// </summary>
      /// <remarks>The method does not modify the file at the specified path. If the file does not exist or
      /// contains invalid JSON, the response will indicate an error.</remarks>
      /// <typeparam name="TData">The type of object to deserialize from the JSON file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the deserialization process.</typeparam>
      /// <param name="filePath">The path to the JSON file to read. Cannot be null or empty.</param>
      /// <param name="options">An object containing options that control the deserialization behavior. Cannot be null.</param>
      /// <returns>A Response TData containing the deserialized object if successful; otherwise, an error response indicating
      /// the failure.</returns>
      public static Response<TData> ReadObjectFromJsonFile<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromJsonFile<TData, TOptions>(filePath, options);

      /// <summary>
      /// Reads a single object from a CSV file using the specified options.
      /// </summary>
      /// <remarks>The method expects the CSV file to contain data compatible with the specified type
      /// parameter. If the file is missing, inaccessible, or contains invalid data, the response will indicate the
      /// error.</remarks>
      /// <typeparam name="TData">The type of object to deserialize from the CSV file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the CSV reading and deserialization process.</typeparam>
      /// <param name="filePath">The path to the CSV file to read. Cannot be null or empty.</param>
      /// <param name="options">An object containing configuration settings for reading and deserializing the CSV file. Cannot be null.</param>
      /// <returns>A Response TData containing the deserialized object and any relevant status or error information.</returns>
      public static Response<TData> ReadObjectFromCsvFile<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromCsvFile<TData, TOptions>(filePath, options);

      /// <summary>
      /// Reads an object of the specified type from an XML file using the provided options.
      /// </summary>
      /// <typeparam name="TData">The type of object to deserialize from the XML file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the XML deserialization process.</typeparam>
      /// <param name="filePath">The path to the XML file containing the serialized object. Cannot be null or empty.</param>
      /// <param name="options">An object containing configuration settings for the XML deserialization. Cannot be null.</param>
      /// <returns>A Response TData containing the deserialized object and any relevant status information. If the file does not
      /// exist or deserialization fails, the response will indicate the error.</returns>
      public static Response<TData> ReadObjectFromXmlFile<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromXmlFile<TData, TOptions>(filePath, options);

      /// <summary>
      /// Reads a YAML file from the specified path and deserializes its contents into an object of the specified type.
      /// </summary>
      /// <typeparam name="TData">The type of object to deserialize from the YAML file.</typeparam>
      /// <typeparam name="TOptions">The type of options used to configure the deserialization process.</typeparam>
      /// <param name="filePath">The path to the YAML file to read. Cannot be null or empty.</param>
      /// <param name="options">An options object that configures how the YAML deserialization is performed. Cannot be null.</param>
      /// <returns>A Response TData containing the deserialized object if successful; otherwise, an error response indicating
      /// the failure.</returns>
      public static Response<TData> ReadObjectFromYamlFile<TData, TOptions>(string filePath, TOptions options)
         => _svc.ReadObjectFromYamlFile<TData, TOptions>(filePath, options);

      /// <summary>
      /// Reads the contents of a text file and returns the result as a string.
      /// </summary>
      /// <param name="filePath">The path to the file to be read. Cannot be null or empty.</param>
      /// <param name="encoding">The character encoding to use when reading the file. If null, the default encoding is used.</param>
      /// <returns>A Response string containing the text read from the file. If the file does not exist or cannot be read, the
      /// response indicates the error.</returns>
      public static Response<string> ReadText(string filePath, Encoding? encoding = null)
         => _svc.ReadText(filePath, encoding);

      /// <summary>
      /// Reads all bytes from the specified file and returns the result in a response object.
      /// </summary>
      /// <param name="filePath">The path to the file to read. Must refer to an existing file; cannot be null or empty.</param>
      /// <returns>A response containing the byte array read from the file. If the file does not exist or cannot be read, the
      /// response may indicate an error.</returns>
      public static Response<byte[]> ReadBytes(string filePath)
         => _svc.ReadBytes(filePath);
   }
}