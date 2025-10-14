using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Afrowave.SharedTools.Ftp.Options;
using Afrowave.SharedTools.Ftp.Static;
using Xunit;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Ftp.Static;

public sealed class FtpClientHelperTests
{
    private static FtpOptions MakeFastFailOptions()
    {
        return new FtpOptions
        {
            Host = "127.0.0.1",
            Port = 21210,
            BasePath = "/",
            Credentials = new NetworkCredential("user", "pass"),
            EnableSsl = false,
            UsePassive = true,
            KeepAlive = false,
            UseBinary = true,
            ConnectTimeoutMs = 500,
            ReadWriteTimeoutMs = 500,
            Retry = new RetryPolicyOptions { MaxRetries = 0, BaseDelay = TimeSpan.FromMilliseconds(10), BackoffFactor = 1.0, Jitter = false }
        };
    }

    [Fact]
    public async Task ListAsync_WhenHostMissing_Throws()
    {
        var opt = new FtpOptions { Host = string.Empty, Credentials = new NetworkCredential() };
        await Assert.ThrowsAsync<ArgumentException>(async () => await FtpClientHelper.ListAsync(opt, "/"));
    }

    [Fact]
    public async Task DownloadBytesAsync_WhenConnectionFails_ReturnsFailResponse()
    {
        var opt = MakeFastFailOptions();
        var res = await FtpClientHelper.DownloadBytesAsync(opt, "/test.bin");
        Assert.False(res.Success);
        Assert.NotNull(res.Message);
    }

    [Fact]
    public async Task ListAsync_WhenConnectionFails_ReturnsFailResponse()
    {
        var opt = MakeFastFailOptions();
        var res = await FtpClientHelper.ListAsync(opt, "/");
        Assert.False(res.Success);
        Assert.NotNull(res.Message);
    }

    [Fact]
    public async Task ExistsAsync_WhenConnectionFails_ReturnsFalse()
    {
        var opt = MakeFastFailOptions();
        var res = await FtpClientHelper.ExistsAsync(opt, "/nope.txt");
        Assert.True(res.Success);
        Assert.False(res.Data);
    }

    [Fact]
    public async Task GetFileSizeAsync_WhenConnectionFails_ReturnsFailResponse()
    {
        var opt = MakeFastFailOptions();
        var res = await FtpClientHelper.GetFileSizeAsync(opt, "/nope.txt");
        Assert.False(res.Success);
    }
}
