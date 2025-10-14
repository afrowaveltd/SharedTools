using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Afrowave.SharedTools.Ftp.Options;
using Afrowave.SharedTools.Ftp.Services;
using Xunit;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Ftp.Services;

public sealed class FtpServiceTests
{
    private static FtpOptions MakeFastFailOptions()
    {
        return new FtpOptions
        {
            Host = "127.0.0.1",   // no FTP server expected; connection refused quickly
            Port = 21210,          // uncommon port to avoid local services
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
    public void Ctor_WhenHostMissing_Throws()
    {
        var opt = new FtpOptions { Host = string.Empty, Credentials = new NetworkCredential() };
        Assert.Throws<ArgumentException>(() => new FtpService(opt));
    }

    [Fact]
    public async Task DownloadBytesAsync_WhenConnectionFails_ReturnsFailResponse()
    {
        var svc = new FtpService(MakeFastFailOptions());
        var res = await svc.DownloadBytesAsync("/test.bin");
        Assert.False(res.Success);
        Assert.NotNull(res.Message);
    }

    [Fact]
    public async Task ListAsync_WhenConnectionFails_ReturnsFailResponse()
    {
        var svc = new FtpService(MakeFastFailOptions());
        var res = await svc.ListAsync("/");
        Assert.False(res.Success);
        Assert.NotNull(res.Message);
    }

    [Fact]
    public async Task ExistsAsync_WhenConnectionFails_ReturnsFalse()
    {
        var svc = new FtpService(MakeFastFailOptions());
        var res = await svc.ExistsAsync("/nope.txt");
        Assert.True(res.Success);
        Assert.False(res.Data);
    }

    [Fact]
    public async Task GetFileSizeAsync_WhenConnectionFails_ReturnsFailResponse()
    {
        var svc = new FtpService(MakeFastFailOptions());
        var res = await svc.GetFileSizeAsync("/nope.txt");
        Assert.False(res.Success);
    }
}
