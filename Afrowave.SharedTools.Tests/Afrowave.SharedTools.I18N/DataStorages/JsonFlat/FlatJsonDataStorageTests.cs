using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Afrowave.SharedTools.I18N.DataStorages.JsonFlat;
using Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Models;
using Afrowave.SharedTools.I18N.DataStorages.Services;
using Afrowave.SharedTools.I18N.EventHandler;
using Afrowave.SharedTools.I18N.Interfaces;
using Afrowave.SharedTools.I18N.Models;
using Afrowave.SharedTools.Models.Results;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.I18N.DataStorages.JsonFlat;

public sealed class FlatJsonDataStorageTests
{
    private static (string path, JsonFlatDataStorageOptions opts) CreateTempOptions(bool createIfNotExists)
    {
        var path = Path.Combine(Path.GetTempPath(), "i18n-jsonflat-" + Guid.NewGuid().ToString("N"));
        return (path, new JsonFlatDataStorageOptions
        {
            LocalesPath = path,
            CreateIfNotExists = createIfNotExists,
            DefaultLanguage = "en"
        });
    }

    private static FlatJsonDataStorage CreateSut(JsonFlatDataStorageOptions opts, ICapabilities? caps = null)
    {
        var optMon = Substitute.For<IOptions<JsonFlatDataStorageOptions>>();
        optMon.Value.Returns(opts);
        if(caps == null)
        {
            caps = Substitute.For<ICapabilities>();
            caps.GetCapabilities().Returns(new DataStorageCapabilities());
        }
        return new FlatJsonDataStorage(optMon, caps);
    }

    [Fact]
    public async Task ListAvailableLanguages_CreatesDirectory_WhenConfigured()
    {
        var (path, cfg) = CreateTempOptions(createIfNotExists: true);
        try
        {
            var sut = CreateSut(cfg);

            var res = await sut.ListAvailableLanguagesAsync();

            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            Assert.Empty(res.Data);
            Assert.True(Directory.Exists(path));
        }
        finally
        {
            if(Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
    }

    [Fact]
    public async Task Save_Then_Load_Roundtrip_Works()
    {
        var (path, cfg) = CreateTempOptions(createIfNotExists: true);
        try
        {
            var sut = CreateSut(cfg);
            var dict = new Dictionary<string, string> { ["hello"] = "world", ["name"] = "Alice" };

            var save = await sut.SaveDictionaryAsync("en", dict);
            Assert.True(save.Success);

            var loaded = await sut.LoadDictionaryAsync("en");
            Assert.True(loaded.Success);
            Assert.NotNull(loaded.Data);
            Assert.Equal("world", loaded.Data["hello"]);
            Assert.Equal("Alice", loaded.Data["name"]);
        }
        finally
        {
            if(Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
    }

    [Fact]
    public async Task DictionaryExists_After_Save_ReturnsTrue_And_Delete_RemovesFile()
    {
        var (path, cfg) = CreateTempOptions(createIfNotExists: true);
        try
        {
            var sut = CreateSut(cfg);
            await sut.SaveDictionaryAsync("cs", new Dictionary<string, string> { ["a"] = "b" });

            var exists = await sut.DictionaryExistsAsync("cs");
            Assert.True(exists);

            var del = await sut.DeleteDictionaryAsync("cs");
            Assert.True(del.Success);

            var existsAfter = await sut.DictionaryExistsAsync("cs");
            Assert.False(existsAfter);
        }
        finally
        {
            if(Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
    }

    [Fact]
    public async Task GetTranslation_ReturnsExpectedValue()
    {
        var (path, cfg) = CreateTempOptions(createIfNotExists: true);
        try
        {
            var sut = CreateSut(cfg);
            await sut.SaveDictionaryAsync("en", new Dictionary<string, string> { ["greet"] = "Hello" });

            var res = await sut.GetTranslation("en", "greet");
            Assert.True(res.Success);
            Assert.Equal("Hello", res.Data);
        }
        finally
        {
            if(Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
    }

    [Fact]
    public async Task Add_Update_Remove_Raises_DictionaryChanged()
    {
        var (path, cfg) = CreateTempOptions(createIfNotExists: true);
        try
        {
            var sut = CreateSut(cfg);
            var events = new List<string>();
            sut.DictionaryChanged += (sender, e) => events.Add(e.LanguageCode);

            var add = await sut.AddTranslation("en", "k1", "v1");
            Assert.True(add.Success);

            var upd = await sut.UpdateTranslation("en", "k1", "v2");
            Assert.True(upd.Success);

            var rem = await sut.RemoveTranslation("en", "k1");
            Assert.True(rem.Success);

            Assert.Equal(3, events.Count);
            Assert.All(events, code => Assert.Equal("en", code));
        }
        finally
        {
            if(Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
    }

    [Fact]
    public async Task IsReadOnlyAsync_Reflects_Capabilities()
    {
        var (path, cfg) = CreateTempOptions(createIfNotExists: true);
        try
        {
            var caps = Substitute.For<ICapabilities>();
            caps.GetCapabilities().Returns(new DataStorageCapabilities { IsReadOnly = true });
            var sut = CreateSut(cfg, caps);

            var ro = await sut.IsReadOnlyAsync();
            Assert.True(ro);
        }
        finally
        {
            if(Directory.Exists(path)) Directory.Delete(path, recursive: true);
        }
    }
}
