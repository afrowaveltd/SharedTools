using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Afrowave.SharedTools.IO.Models;
using Afrowave.SharedTools.IO.Services;
using CsvHelper.Configuration;
using Xunit;
using System.Xml;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.IO.Services;

public sealed class FileServiceTests
{
    public sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private static string NewTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "io-fileservice-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Fact]
    public async Task Json_ReadWrite_Roundtrip_Async()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.json");
            var model = new Person { Name = "Alice", Age = 30 };

            var w = await svc.StoreObjectToJsonFileAsync(model, file, options: (JsonSerializerOptions?)null);
            Assert.True(w.Success);

            var r = await svc.ReadObjectFromJsonFileAsync<Person, JsonSerializerOptions?>(file, options: null);
            Assert.True(r.Success);
            Assert.Equal("Alice", r.Data.Name);
            Assert.Equal(30, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Json_ReadWrite_Roundtrip_Sync()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.json");
            var model = new Person { Name = "Sync", Age = 11 };

            var w = svc.StoreObjectToJsonFile(model, file, options: (JsonSerializerOptions?)null);
            Assert.True(w.Success);

            var r = svc.ReadObjectFromJsonFile<Person, JsonSerializerOptions?>(file, options: null);
            Assert.True(r.Success);
            Assert.Equal("Sync", r.Data.Name);
            Assert.Equal(11, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Xml_ReadWrite_Roundtrip_Async()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.xml");
            var model = new Person { Name = "Bob", Age = 25 };

            var w = await svc.StoreObjectToXmlFileAsync(model, file, options: (XmlWriterSettings?)null);
            Assert.True(w.Success);

            var r = await svc.ReadObjectFromXmlFileAsync<Person, XmlReaderSettings?>(file, options: null);
            Assert.True(r.Success);
            Assert.Equal("Bob", r.Data.Name);
            Assert.Equal(25, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Xml_ReadWrite_Roundtrip_Sync()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.xml");
            var model = new Person { Name = "SyncXml", Age = 15 };

            var w = svc.StoreObjectToXmlFile(model, file, options: (XmlWriterSettings?)null);
            Assert.True(w.Success);

            var r = svc.ReadObjectFromXmlFile<Person, XmlReaderSettings?>(file, options: null);
            Assert.True(r.Success);
            Assert.Equal("SyncXml", r.Data.Name);
            Assert.Equal(15, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Yaml_ReadWrite_Roundtrip_Async()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.yaml");
            var model = new Person { Name = "Carol", Age = 42 };

            var w = await svc.StoreObjectToFileAsync(model, file, FileType.Yaml, options: (object?)null);
            Assert.True(w.Success);

            var r = await svc.ReadObjectFromFileAsync<Person, object?>(file, FileType.Yaml, options: null);
            Assert.True(r.Success);
            Assert.Equal("Carol", r.Data.Name);
            Assert.Equal(42, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Yaml_ReadWrite_Roundtrip_Sync()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.yaml");
            var model = new Person { Name = "YSync", Age = 33 };

            var w = svc.StoreObjectToFile(model, file, FileType.Yaml, options: (object?)null);
            Assert.True(w.Success);

            var r = svc.ReadObjectFromFile<Person, object?>(file, FileType.Yaml, options: null);
            Assert.True(r.Success);
            Assert.Equal("YSync", r.Data.Name);
            Assert.Equal(33, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Csv_List_ReadWrite_Roundtrip_Async()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "people.csv");
            var list = new List<Person>
            {
                new Person { Name = "A", Age = 1 },
                new Person { Name = "B", Age = 2 },
            };

            var w = await svc.StoreObjectToCsvFileAsync(list, file, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
            Assert.True(w.Success);

            var r = await svc.ReadObjectFromCsvFileAsync<List<Person>, CsvConfiguration>(file, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
            Assert.True(r.Success);
            Assert.Equal(2, r.Data.Count);
            Assert.Equal("A", r.Data[0].Name);
            Assert.Equal(2, r.Data[1].Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Csv_List_ReadWrite_Roundtrip_Sync()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "people.csv");
            var list = new List<Person>
            {
                new Person { Name = "C", Age = 3 },
                new Person { Name = "D", Age = 4 },
            };

            var w = svc.StoreObjectToCsvFile(list, file, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
            Assert.True(w.Success);

            var r = svc.ReadObjectFromCsvFile<List<Person>, CsvConfiguration>(file, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
            Assert.True(r.Success);
            Assert.Equal(2, r.Data.Count);
            Assert.Equal("C", r.Data[0].Name);
            Assert.Equal(4, r.Data[1].Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Text_Bytes_ReadWrite_Async()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var tf = Path.Combine(tmp, "note.txt");
            var bf = Path.Combine(tmp, "bin.dat");

            var wt = await svc.WriteTextAsync(tf, "Hello", Encoding.UTF8);
            Assert.True(wt.Success);
            var rt = await svc.ReadTextAsync(tf, Encoding.UTF8);
            Assert.True(rt.Success);
            Assert.Equal("Hello", rt.Data);

            var bytes = new byte[] { 1, 2, 3, 4 };
            var wb = await svc.WriteBytesAsync(bf, bytes);
            Assert.True(wb.Success);
            var rb = await svc.ReadBytesAsync(bf);
            Assert.True(rb.Success);
            Assert.Equal(bytes, rb.Data);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Text_Bytes_ReadWrite_Sync()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var tf = Path.Combine(tmp, "note.txt");
            var bf = Path.Combine(tmp, "bin.dat");

            var wt = svc.WriteText(tf, "Hi", Encoding.UTF8);
            Assert.True(wt.Success);
            var rt = svc.ReadText(tf, Encoding.UTF8);
            Assert.True(rt.Success);
            Assert.Equal("Hi", rt.Data);

            var bytes = new byte[] { 5, 6 };
            var wb = svc.WriteBytes(bf, bytes);
            Assert.True(wb.Success);
            var rb = svc.ReadBytes(bf);
            Assert.True(rb.Success);
            Assert.Equal(bytes, rb.Data);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Router_SaveLoad_Json_Works()
    {
        var svc = new FileService();
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "router.json");
            var model = new Person { Name = "Router", Age = 77 };

            var w = await svc.StoreObjectToFileAsync(model, file, FileType.Json, (JsonSerializerOptions?)null);
            Assert.True(w.Success);

            var r = await svc.ReadObjectFromFileAsync<Person, JsonSerializerOptions?>(file, FileType.Json, null);
            Assert.True(r.Success);
            Assert.Equal("Router", r.Data.Name);
            Assert.Equal(77, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }
}
