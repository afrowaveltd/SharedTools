using Afrowave.SharedTools.IO.Models;
using Afrowave.SharedTools.Models.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Afrowave.SharedTools.IO.Services
{
   /// <summary>
   /// Provides high-level file IO operations for text, binary, and object serialization.
   /// </summary>
   /// <remarks>
   /// Supports JSON, XML, CSV, and YAML formats. Defaults use UTF-8 without BOM, JSON/YAML camelCase,
   /// XML indented output, and CSV with header and invariant culture. <c>TOptions</c> parameters can supply
   /// native serializer options to override defaults.
   /// </remarks>
   public class FileService : IFileService
   {
      private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

      // --------- public API (async) ---------

      // Storing
      /// <summary>
      /// Serializes the specified object as JSON and writes it to the given file path.
      /// </summary>
      public async Task<Result> StoreObjectToJsonFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var opts = BuildJsonOptions(options);
            var text = SerializeJson(obj, opts);
            await WriteTextFileAsync(filePath, text).ConfigureAwait(false);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Serializes the specified object as XML and writes it to the given file path.
      /// </summary>
      public async Task<Result> StoreObjectToXmlFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var settings = BuildXmlWriterSettings(options);
            var text = SerializeXml(obj, settings);
            await WriteTextFileAsync(filePath, text).ConfigureAwait(false);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Serializes the specified object as CSV and writes it to the given file path.
      /// </summary>
      public async Task<Result> StoreObjectToCsvFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var cfg = BuildCsvConfig(options);
            var text = SerializeCsv(obj, cfg);
            await WriteTextFileAsync(filePath, text).ConfigureAwait(false);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Serializes the specified object to a file in the requested format.
      /// </summary>
      public async Task<Result> StoreObjectToFileAsync<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options)
      {
         return fileType switch
         {
            FileType.Json => await StoreObjectToJsonFileAsync(obj, filePath, options).ConfigureAwait(false),
            FileType.Xml => await StoreObjectToXmlFileAsync(obj, filePath, options).ConfigureAwait(false),
            FileType.Csv => await StoreObjectToCsvFileAsync(obj, filePath, options).ConfigureAwait(false),
            FileType.Yaml => await StoreObjectToYamlFileAsync(obj, filePath, options).ConfigureAwait(false),
            _ => Result.Fail("Unsupported file type")
         };
      }

      /// <summary>
      /// Writes the specified text to a file using the provided or default encoding.
      /// </summary>
      public async Task<Result> WriteTextAsync(string filePath, string content, Encoding? encoding = null)
      {
         try
         {
            await WriteTextFileAsync(filePath, content, encoding).ConfigureAwait(false);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Writes raw bytes to the specified file path.
      /// </summary>
      public async Task<Result> WriteBytesAsync(string filePath, byte[] data)
      {
         try
         {
            await WriteBytesFileAsync(filePath, data).ConfigureAwait(false);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      // reading

      /// <summary>
      /// Reads and deserializes the object from a file according to the specified format.
      /// </summary>
      public async Task<Response<TData>> ReadObjectFromFileAsync<TData, TOptions>(string filePath, FileType fileType, TOptions options)
      {
         return fileType switch
         {
            FileType.Json => await ReadObjectFromJsonFileAsync<TData, TOptions>(filePath, options).ConfigureAwait(false),
            FileType.Xml => await ReadObjectFromXmlFileAsync<TData, TOptions>(filePath, options).ConfigureAwait(false),
            FileType.Csv => await ReadObjectFromCsvFileAsync<TData, TOptions>(filePath, options).ConfigureAwait(false),
            FileType.Yaml => await ReadObjectFromYamlFileAsync<TData, TOptions>(filePath, options).ConfigureAwait(false),
            _ => Response<TData>.Fail("Unsupported file type")
         };
      }

      /// <summary>
      /// Reads and deserializes JSON content from a file.
      /// </summary>
      public async Task<Response<TData>> ReadObjectFromJsonFileAsync<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = await ReadTextFileAsync(filePath).ConfigureAwait(false);
            var opts = BuildJsonOptions(options);
            var data = DeserializeJson<TData>(text, opts);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads and deserializes CSV content from a file.
      /// </summary>
      public async Task<Response<TData>> ReadObjectFromCsvFileAsync<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = await ReadTextFileAsync(filePath).ConfigureAwait(false);
            var cfg = BuildCsvConfig(options);
            var data = DeserializeCsv<TData>(text, cfg);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads and deserializes XML content from a file.
      /// </summary>
      public async Task<Response<TData>> ReadObjectFromXmlFileAsync<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = await ReadTextFileAsync(filePath).ConfigureAwait(false);
            var xr = BuildXmlReaderSettings(options);
            var data = DeserializeXml<TData>(text, xr);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads and deserializes YAML content from a file.
      /// </summary>
      public async Task<Response<TData>> ReadObjectFromYamlFileAsync<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = await ReadTextFileAsync(filePath).ConfigureAwait(false);
            var deserializer = BuildYamlDeserializer(options);
            var data = deserializer.Deserialize<TData>(text);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads the entire file as text using the specified or default encoding.
      /// </summary>
      public async Task<Response<string>> ReadTextAsync(string filePath, Encoding? encoding = null)
      {
         try
         {
            var content = await ReadTextFileAsync(filePath, encoding).ConfigureAwait(false);
            return Response<string>.SuccessResponse(content, "OK");
         }
         catch(Exception ex)
         {
            return Response<string>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads the entire file as raw bytes.
      /// </summary>
      public async Task<Response<byte[]>> ReadBytesAsync(string filePath)
      {
         try
         {
            var content = await ReadBytesFileAsync(filePath).ConfigureAwait(false);
            return Response<byte[]>.SuccessResponse(content, "OK");
         }
         catch(Exception ex)
         {
            return Response<byte[]>.Fail(ex);
         }
      }

      // --------- public API (sync) ---------

      // Storing
      /// <summary>
      /// Serializes the specified object as JSON and writes it to the given file path.
      /// </summary>
      public Result StoreObjectToJsonFile<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var opts = BuildJsonOptions(options);
            var text = SerializeJson(obj, opts);
            WriteTextFile(filePath, text);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Serializes the specified object as XML and writes it to the given file path.
      /// </summary>
      public Result StoreObjectToXmlFile<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var settings = BuildXmlWriterSettings(options);
            var text = SerializeXml(obj, settings);
            WriteTextFile(filePath, text);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Serializes the specified object as CSV and writes it to the given file path.
      /// </summary>
      public Result StoreObjectToCsvFile<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var cfg = BuildCsvConfig(options);
            var text = SerializeCsv(obj, cfg);
            WriteTextFile(filePath, text);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Serializes the specified object to a file in the requested format.
      /// </summary>
      public Result StoreObjectToFile<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options)
      {
         return fileType switch
         {
            FileType.Json => StoreObjectToJsonFile(obj, filePath, options),
            FileType.Xml => StoreObjectToXmlFile(obj, filePath, options),
            FileType.Csv => StoreObjectToCsvFile(obj, filePath, options),
            FileType.Yaml => StoreObjectToYamlFile(obj, filePath, options),
            _ => Result.Fail("Unsupported file type")
         };
      }

      /// <summary>
      /// Writes the specified text to a file using the provided or default encoding.
      /// </summary>
      public Result WriteText(string filePath, string content, Encoding? encoding = null)
      {
         try
         {
            WriteTextFile(filePath, content, encoding);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      /// <summary>
      /// Writes raw bytes to the specified file path.
      /// </summary>
      public Result WriteBytes(string filePath, byte[] data)
      {
         try
         {
            WriteBytesFile(filePath, data);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      // reading

      /// <summary>
      /// Reads and deserializes the object from a file according to the specified format.
      /// </summary>
      public Response<TData> ReadObjectFromFile<TData, TOptions>(string filePath, FileType fileType, TOptions options)
      {
         return fileType switch
         {
            FileType.Json => ReadObjectFromJsonFile<TData, TOptions>(filePath, options),
            FileType.Xml => ReadObjectFromXmlFile<TData, TOptions>(filePath, options),
            FileType.Csv => ReadObjectFromCsvFile<TData, TOptions>(filePath, options),
            FileType.Yaml => ReadObjectFromYamlFile<TData, TOptions>(filePath, options),
            _ => Response<TData>.Fail("Unsupported file type")
         };
      }

      /// <summary>
      /// Reads and deserializes JSON content from a file.
      /// </summary>
      public Response<TData> ReadObjectFromJsonFile<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = ReadTextFile(filePath);
            var opts = BuildJsonOptions(options);
            var data = DeserializeJson<TData>(text, opts);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads and deserializes CSV content from a file.
      /// </summary>
      public Response<TData> ReadObjectFromCsvFile<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = ReadTextFile(filePath);
            var cfg = BuildCsvConfig(options);
            var data = DeserializeCsv<TData>(text, cfg);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads and deserializes XML content from a file.
      /// </summary>
      public Response<TData> ReadObjectFromXmlFile<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = ReadTextFile(filePath);
            var xr = BuildXmlReaderSettings(options);
            var data = DeserializeXml<TData>(text, xr);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads and deserializes YAML content from a file.
      /// </summary>
      public Response<TData> ReadObjectFromYamlFile<TData, TOptions>(string filePath, TOptions options)
      {
         try
         {
            var text = ReadTextFile(filePath);
            var deserializer = BuildYamlDeserializer(options);
            var data = deserializer.Deserialize<TData>(text);
            return Response<TData>.SuccessResponse(data, "OK");
         }
         catch(Exception ex)
         {
            return Response<TData>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads the entire file as text using the specified or default encoding.
      /// </summary>
      public Response<string> ReadText(string filePath, Encoding? encoding = null)
      {
         try
         {
            var content = ReadTextFile(filePath, encoding);
            return Response<string>.SuccessResponse(content, "OK");
         }
         catch(Exception ex)
         {
            return Response<string>.Fail(ex);
         }
      }

      /// <summary>
      /// Reads the entire file as raw bytes.
      /// </summary>
      public Response<byte[]> ReadBytes(string filePath)
      {
         try
         {
            var content = ReadBytesFile(filePath);
            return Response<byte[]>.SuccessResponse(content, "OK");
         }
         catch(Exception ex)
         {
            return Response<byte[]>.Fail(ex);
         }
      }

      // --------- private helpers (at bottom) ---------

      private static JsonSerializerOptions BuildJsonOptions<TOptions>(TOptions options)
      {
         if(options is JsonSerializerOptions j) return j;
         return new JsonSerializerOptions
         {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
#pragma warning disable SYSLIB0020
            IgnoreNullValues = true,
#pragma warning restore SYSLIB0020
            WriteIndented = true
         };
      }

      private static string SerializeJson<T>(T obj, JsonSerializerOptions opts)
         => JsonSerializer.Serialize(obj, opts);

      private static T DeserializeJson<T>(string json, JsonSerializerOptions opts)
         => JsonSerializer.Deserialize<T>(json, opts);

      private static XmlWriterSettings BuildXmlWriterSettings<TOptions>(TOptions options)
      {
         if(options is XmlWriterSettings xw) return xw;
         return new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false, Encoding = Utf8NoBom };
      }

      private static XmlReaderSettings BuildXmlReaderSettings<TOptions>(TOptions options)
      {
         if(options is XmlReaderSettings xr) return xr;
         return new XmlReaderSettings();
      }

      private static string SerializeXml<T>(T obj, XmlWriterSettings settings)
      {
         var serializer = new XmlSerializer(typeof(T));
         using var sw = new StringWriter();
         using(var xw = XmlWriter.Create(sw, settings))
         {
            serializer.Serialize(xw, obj);
         }
         return sw.ToString();
      }

      private static T DeserializeXml<T>(string xml, XmlReaderSettings settings)
      {
         var serializer = new XmlSerializer(typeof(T));
         using var sr = new StringReader(xml);
         using var xr = XmlReader.Create(sr, settings);
         return (T)serializer.Deserialize(xr);
      }

      private static CsvConfiguration BuildCsvConfig<TOptions>(TOptions options)
      {
         if(options is CsvConfiguration cfg) return cfg;
         var cfgDefault = new CsvConfiguration(CultureInfo.InvariantCulture)
         {
            Delimiter = ",",
            HasHeaderRecord = true,
            Encoding = Utf8NoBom,
            TrimOptions = TrimOptions.Trim
         };
         return cfgDefault;
      }

      private static string SerializeCsv<T>(T obj, CsvConfiguration cfg)
      {
         using var sw = new StringWriter();
         using var csv = new CsvWriter(sw, cfg);
         if(obj is System.Collections.IEnumerable seq && !(obj is string))
         {
            csv.WriteRecords(seq);
         }
         else
         {
            csv.WriteHeader<T>();
            csv.NextRecord();
            csv.WriteRecord(obj);
            csv.NextRecord();
         }
         return sw.ToString();
      }

      private static T DeserializeCsv<T>(string content, CsvConfiguration cfg)
      {
         using var sr = new StringReader(content);
         using var csv = new CsvReader(sr, cfg);

         var t = typeof(T);
         if(t.IsGenericType)
         {
            var def = t.GetGenericTypeDefinition();
            if(def == typeof(List<>) || def == typeof(IList<>) || def == typeof(IEnumerable<>))
            {
               var itemType = t.GetGenericArguments()[0];
               var method = typeof(FileService).GetMethod(nameof(ReadCsvAsList), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
               var generic = method.MakeGenericMethod(itemType);
               var list = generic.Invoke(null, new object[] { csv });
               return (T)list;
            }
         }

         return csv.GetRecord<T>();
      }

      private static List<T> ReadCsvAsList<T>(CsvReader csv)
      {
         var list = new List<T>();
         foreach(var rec in csv.GetRecords<T>())
            list.Add(rec);
         return list;
      }

      private static ISerializer BuildYamlSerializer<TOptions>(TOptions options)
      {
         if(options is ISerializer s) return s;
         return new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
      }

      private static IDeserializer BuildYamlDeserializer<TOptions>(TOptions options)
      {
         if(options is IDeserializer d) return d;
         return new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
      }

      private static async Task WriteTextFileAsync(string path, string content, Encoding? encoding = null)
      {
         EnsureDirectory(path);
         var enc = encoding ?? Utf8NoBom;
         await File.WriteAllTextAsync(path, content, enc).ConfigureAwait(false);
      }

      private static void WriteTextFile(string path, string content, Encoding? encoding = null)
      {
         EnsureDirectory(path);
         var enc = encoding ?? Utf8NoBom;
         File.WriteAllText(path, content, enc);
      }

      private static async Task<string> ReadTextFileAsync(string path, Encoding? encoding = null)
      {
         var enc = encoding ?? Utf8NoBom;
         return await File.ReadAllTextAsync(path, enc).ConfigureAwait(false);
      }

      private static string ReadTextFile(string path, Encoding? encoding = null)
      {
         var enc = encoding ?? Utf8NoBom;
         return File.ReadAllText(path, enc);
      }

      private static async Task WriteBytesFileAsync(string path, byte[] data)
      {
         EnsureDirectory(path);
         await File.WriteAllBytesAsync(path, data).ConfigureAwait(false);
      }

      private static void WriteBytesFile(string path, byte[] data)
      {
         EnsureDirectory(path);
         File.WriteAllBytes(path, data);
      }

      private static async Task<byte[]> ReadBytesFileAsync(string path)
      {
         return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
      }

      private static byte[] ReadBytesFile(string path)
      {
         return File.ReadAllBytes(path);
      }

      private static void EnsureDirectory(string filePath)
      {
         var dir = Path.GetDirectoryName(filePath);
         if(!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
      }

      private async Task<Result> StoreObjectToYamlFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var serializer = BuildYamlSerializer(options);
            var text = serializer.Serialize(obj);
            await WriteTextFileAsync(filePath, text).ConfigureAwait(false);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }

      private Result StoreObjectToYamlFile<TData, TOptions>(TData obj, string filePath, TOptions options)
      {
         try
         {
            var serializer = BuildYamlSerializer(options);
            var text = serializer.Serialize(obj);
            WriteTextFile(filePath, text);
            return Result.Ok();
         }
         catch(Exception ex)
         {
            return Result.Fail(ex.Message);
         }
      }
   }
}