using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Afrowave.SharedTools.IO.Models;
using Afrowave.SharedTools.IO.Static;
using CsvHelper.Configuration;
using Xunit;
using System.Xml;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.IO.Static;

public sealed class FileHelperTests
{
    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private static string NewTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "io-filehelper-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    [Fact]
    public async Task Json_ReadWrite_Roundtrip_Async()
    {
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.json");
            var model = new Person { Name = "Alice", Age = 30 };

            var w = await FileHelper.StoreObjectToJsonFileAsync(model, file, options: (JsonSerializerOptions?)null);
            Assert.True(w.Success);

            var r = await FileHelper.ReadObjectFromJsonFileAsync<Person, JsonSerializerOptions?>(file, options: null);
            Assert.True(r.Success);
            Assert.Equal("Alice", r.Data.Name);
            Assert.Equal(30, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Json_ReadWrite_Roundtrip_Sync()
    {
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "person.json");
            var model = new Person { Name = "Sync", Age = 11 };

            var w = FileHelper.StoreObjectToJsonFile(model, file, options: (JsonSerializerOptions?)null);
            Assert.True(w.Success);

            var r = FileHelper.ReadObjectFromJsonFile<Person, JsonSerializerOptions?>(file, options: null);
            Assert.True(r.Success);
            Assert.Equal("Sync", r.Data.Name);
            Assert.Equal(11, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Router_Yaml_Roundtrip_Async()
    {
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "p.yaml");
            var model = new Person { Name = "X", Age = 9 };

            var w = await FileHelper.StoreObjectToFileAsync(model, file, FileType.Yaml, options: (object?)null);
            Assert.True(w.Success);

            var r = await FileHelper.ReadObjectFromFileAsync<Person, object?>(file, FileType.Yaml, options: null);
            Assert.True(r.Success);
            Assert.Equal("X", r.Data.Name);
            Assert.Equal(9, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Router_Yaml_Roundtrip_Sync()
    {
        var tmp = NewTempDir();
        try
        {
            var file = Path.Combine(tmp, "p.yaml");
            var model = new Person { Name = "XS", Age = 10 };

            var w = FileHelper.StoreObjectToFile(model, file, FileType.Yaml, options: (object?)null);
            Assert.True(w.Success);

            var r = FileHelper.ReadObjectFromFile<Person, object?>(file, FileType.Yaml, options: null);
            Assert.True(r.Success);
            Assert.Equal("XS", r.Data.Name);
            Assert.Equal(10, r.Data.Age);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public async Task Text_Bytes_ReadWrite_Async()
    {
        var tmp = NewTempDir();
        try
        {
            var tf = Path.Combine(tmp, "note.txt");
            var bf = Path.Combine(tmp, "bin.dat");

            var wt = await FileHelper.WriteTextAsync(tf, "Hello", Encoding.UTF8);
            Assert.True(wt.Success);
            var rt = await FileHelper.ReadTextAsync(tf, Encoding.UTF8);
            Assert.True(rt.Success);
            Assert.Equal("Hello", rt.Data);

            var bytes = new byte[] { 10, 20, 30 };
            var wb = await FileHelper.WriteBytesAsync(bf, bytes);
            Assert.True(wb.Success);
            var rb = await FileHelper.ReadBytesAsync(bf);
            Assert.True(rb.Success);
            Assert.Equal(bytes, rb.Data);
        }
        finally { Directory.Delete(tmp, true); }
    }

    [Fact]
    public void Text_Bytes_ReadWrite_Sync()
    {
        var tmp = NewTempDir();
        try
        {
            var tf = Path.Combine(tmp, "note.txt");
            var bf = Path.Combine(tmp, "bin.dat");

            var wt = FileHelper.WriteText(tf, "Hi", Encoding.UTF8);
            Assert.True(wt.Success);
            var rt = FileHelper.ReadText(tf, Encoding.UTF8);
            Assert.True(rt.Success);
            Assert.Equal("Hi", rt.Data);

            var bytes = new byte[] { 5, 6 };
            var wb = FileHelper.WriteBytes(bf, bytes);
            Assert.True(wb.Success);
            var rb = FileHelper.ReadBytes(bf);
            Assert.True(rb.Success);
            Assert.Equal(bytes, rb.Data);
        }
        finally { Directory.Delete(tmp, true); }
    }
}
