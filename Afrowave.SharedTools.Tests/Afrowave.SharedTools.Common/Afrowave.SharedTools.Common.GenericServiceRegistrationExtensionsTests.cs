using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Afrowave.SharedTools.Common;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Common;

public class Afrowave_SharedTools_Common_GenericServiceRegistrationExtensionsTests
{
	// Test helpers for DI
	public interface ITestService { }

	public class TestService_Options : ITestService
	{
		public TestOptions Options { get; }
		public TestService_Options(IOptions<TestOptions> options)
		{
			Options = options.Value;
		}
	}

	public class TestService_OptionsMonitor : ITestService
	{
		public TestOptions Options { get; }
		public TestService_OptionsMonitor(IOptionsMonitor<TestOptions> monitor)
		{
			Options = monitor.CurrentValue;
		}
	}

	public class TestOptions
	{
		public string Name { get; set; } = string.Empty;
		public int Number { get; set; }
	}

	[Fact]
	public void AddConfiguredService_WithLambda_Registers_Service_And_Options()
	{
		var services = new ServiceCollection();

		services.AddConfiguredService<ITestService, TestService_Options, TestOptions>(
			opts => { opts.Name = "from-lambda"; opts.Number = 7; },
			ServiceLifetime.Singleton);

		using var provider = services.BuildServiceProvider();
		var svc = Assert.IsType<TestService_Options>(provider.GetRequiredService<ITestService>());
		var options = provider.GetRequiredService<IOptions<TestOptions>>().Value;

		Assert.Equal("from-lambda", svc.Options.Name);
		Assert.Equal(7, svc.Options.Number);
		Assert.Equal("from-lambda", options.Name);
		Assert.Equal(7, options.Number);
	}

	[Fact]
	public void AddConfiguredService_WithConfiguration_Binds_And_Resolves()
	{
		var data = new Dictionary<string, string?>
		{
			["Name"] = "from-config",
			["Number"] = "42"
		};
		IConfiguration cfg = new ConfigurationBuilder().AddInMemoryCollection(data!).Build();

		var services = new ServiceCollection();
		services.AddConfiguredService<ITestService, TestService_OptionsMonitor, TestOptions>(
			cfg, ServiceLifetime.Transient);

		using var provider = services.BuildServiceProvider();
		var opts = provider.GetRequiredService<IOptions<TestOptions>>().Value;
		Assert.Equal("from-config", opts.Name);
		Assert.Equal(42, opts.Number);
	}

	[Fact]
	public void Lifetime_Singleton_Scoped_Transient_Behavior()
	{
		var services = new ServiceCollection();

		services.AddConfiguredService<ITestService, TestService_Options, TestOptions>(
			o => { }, ServiceLifetime.Singleton);
		services.AddConfiguredService<ITestService, TestService_Options, TestOptions>(
			o => { }, ServiceLifetime.Singleton); // idempotent registration semantics

		using var providerSingleton = services.BuildServiceProvider();
		var s1 = providerSingleton.GetRequiredService<ITestService>();
		var s2 = providerSingleton.GetRequiredService<ITestService>();
		Assert.Same(s1, s2);

		var servicesScoped = new ServiceCollection();
		servicesScoped.AddConfiguredService<ITestService, TestService_Options, TestOptions>(o => { }, ServiceLifetime.Scoped);
		using var providerScoped = servicesScoped.BuildServiceProvider();
		using var scopeA = providerScoped.CreateScope();
		using var scopeB = providerScoped.CreateScope();
		var a1 = scopeA.ServiceProvider.GetRequiredService<ITestService>();
		var a2 = scopeA.ServiceProvider.GetRequiredService<ITestService>();
		var b1 = scopeB.ServiceProvider.GetRequiredService<ITestService>();
		Assert.Same(a1, a2);
		Assert.NotSame(a1, b1);

		var servicesTransient = new ServiceCollection();
		servicesTransient.AddConfiguredService<ITestService, TestService_Options, TestOptions>(o => { }, ServiceLifetime.Transient);
		using var providerTransient = servicesTransient.BuildServiceProvider();
		var t1 = providerTransient.GetRequiredService<ITestService>();
		var t2 = providerTransient.GetRequiredService<ITestService>();
		Assert.NotSame(t1, t2);
	}

	[Fact]
	public void Validate_Predicate_Is_Invoked_And_Throws_On_Invalid()
	{
		var validate = Substitute.For<Func<TestOptions, bool>>();
		validate.Invoke(Arg.Any<TestOptions>()).Returns(false);

		var services = new ServiceCollection();
		services.AddConfiguredService<ITestService, TestService_Options, TestOptions>(
			o => { o.Name = "x"; },
			ServiceLifetime.Singleton,
			validate,
			"bad options");

		using var provider = services.BuildServiceProvider();

		// Accessing Value should trigger validation and throw
		var ex = Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<TestOptions>>().Value);
		Assert.Contains("bad options", ex.Message, StringComparison.OrdinalIgnoreCase);

		// Ensure the predicate has been called at least once
		validate.Received().Invoke(Arg.Any<TestOptions>());
	}

	[Fact]
	public void AddServiceProfile_Registers_Named_Options_And_Service()
	{
		var services = new ServiceCollection();
		services.AddServiceProfile<ITestService, TestService_Options, TestOptions>("A", o => { o.Name = "A"; o.Number = 1; });
		services.AddServiceProfile<ITestService, TestService_Options, TestOptions>("B", o => { o.Name = "B"; o.Number = 2; });

		using var provider = services.BuildServiceProvider();
		var monitor = provider.GetRequiredService<IOptionsMonitor<TestOptions>>();
		var a = monitor.Get("A");
		var b = monitor.Get("B");

		Assert.Equal("A", a.Name);
		Assert.Equal(1, a.Number);
		Assert.Equal("B", b.Name);
		Assert.Equal(2, b.Number);

		// Service must be resolvable (registered once regardless of number of profiles)
		var service = provider.GetRequiredService<ITestService>();
		Assert.IsAssignableFrom<ITestService>(service);
	}
}
