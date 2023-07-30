﻿// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     PlaywrightWebApplicationFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

using Microsoft.AspNetCore.TestHost;

using Xunit.Sdk;

namespace BlazorBlog.Web.WebTests.Fixtures;

/// <summary>
///   WebApplicationFactory that wraps the TestHost in a Kestrel server.
///   <p>
///     Credit to <a href="https://github.com/CZEMacLeod">https://github.com/CZEMacLeod</a> for writing this.
///   </p>
/// </summary>
[ExcludeFromCodeCoverage]
public class PlaywrightWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
	where TProgram : class
{
	private static int _nextPort;
	private readonly IMessageSink _output;
	private IBrowser? _browser;
	private IPlaywright? _playwright;
	private string? _uri;

	public PlaywrightWebApplicationFactory(IMessageSink output)
	{
		_output = output;
	}

	public string? Uri => _uri;

	[MemberNotNull(nameof(_playwright), nameof(_browser))]
	public async Task InitializeAsync()
	{
		PlaywrightUtilities.InstallPlaywright(BrowserType);
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
		_playwright ??= await Playwright.CreateAsync() ?? throw new InvalidOperationException();
		_browser ??= await GetBrowser().LaunchAsync(LaunchOptions) ?? throw new InvalidOperationException();
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
	}

	async Task IAsyncLifetime.DisposeAsync()
	{
		if (_browser is not null)
		{
			await _browser.DisposeAsync();
		}

		_browser = null;
		_playwright?.Dispose();
		_playwright = null;
	}

	protected virtual IBrowserType GetBrowser()
	{
		return BrowserType switch
		{
			PlaywrightBrowserType.Chromium => _playwright?.Chromium,
			PlaywrightBrowserType.Firefox => _playwright?.Firefox,
			PlaywrightBrowserType.Webkit => _playwright?.Webkit,
			_ => throw new ArgumentOutOfRangeException(nameof(BrowserType))
		} ?? throw new InvalidOperationException("Could not get browser type");
	}

	[MemberNotNull(nameof(_uri))]
	protected override IHost CreateHost(IHostBuilder builder)
	{
		if (Environment is not null)
		{
			builder.UseEnvironment(Environment);
		}

		builder.ConfigureLogging(logging =>
		{
			logging.SetMinimumLevel(MinimumLogLevel);
			if (AddMessageSinkProvider)
			{
				logging.AddProvider(new MessageSinkProvider(_output));
			}
		});

		// We randomize the server port so we ensure that any hard coded Uri's fail in the tests.
		// This also allows multiple servers to run during the tests.
		int port = 5000 + Interlocked.Add(ref _nextPort, 10 + Random.Shared.Next(10));
		_uri = $"http://localhost:{port}";

		// We the testHost, which can be used with HttpClient with a custom transport
		// It is assumed that the return of CreateHost is a host based on the TestHost Server.
		IHost testHost = base.CreateHost(builder);

		// Now we reconfigure the builder to use kestrel so we have an http listener that can be used by playwright
		builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel(options =>
		{
			options.ListenLocalhost(port);
		}));
		IHost host = base.CreateHost(builder);

		UpdateUriFromHost(host); // For some reason, the kestrel server host does not seem to return the addresses.

		return new CompositeHost(testHost, host);
	}

	private void UpdateUriFromHost(IHost host)
	{
		IServer server = host.Services.GetRequiredService<IServer>();
		IServerAddressesFeature addresses = server.Features.Get<IServerAddressesFeature>() ??
		                                    throw new NullReferenceException("Could not get IServerAddressesFeature");
		string? serverAddress = addresses.Addresses.FirstOrDefault();

		if (serverAddress is not null)
		{
			_uri = serverAddress;
		}
		else
		{
			DiagnosticMessage message = new("Could not get server address from IServerAddressesFeature");
			_output.OnMessage(message);
		}
	}

	public async Task<IPage> CreatePlaywrightPageAsync()
	{
		TestServer _server = Server; // Ensure Server is initialized
		await InitializeAsync(); // Ensure Playwright is initialized

		return await _browser.NewPageAsync(new BrowserNewPageOptions { BaseURL = _uri });
	}

	/// <summary>
	///   Install and deploy all binaries Playwright may need.
	/// </summary>
	private static void ShowTrace(string traceName)
	{
		int exitCode = Program.Main(
			new[] { "install-deps" });
		if (exitCode != 0)
		{
			throw new Exception(
				$"Playwright exited with code {exitCode} on install-deps");
		}

		exitCode = Program.Main(new[] { "install" });
		if (exitCode != 0)
		{
			throw new Exception(
				$"Playwright exited with code {exitCode} on install");
		}
	}

	// CompositeHost is based on https://github.com/xaviersolau/DevArticles/blob/e2e_test_blazor_with_playwright/MyBlazorApp/MyAppTests/WebTestingHostFactory.cs
	// Relay the call to both test host and kestrel host.
	public class CompositeHost : IHost
	{
		private readonly IHost kestrelHost;
		private readonly IHost testHost;

		public CompositeHost(IHost testHost, IHost kestrelHost)
		{
			this.testHost = testHost;
			this.kestrelHost = kestrelHost;
		}

		public IServiceProvider Services => testHost.Services;

		public void Dispose()
		{
			testHost.Dispose();
			kestrelHost.Dispose();
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			await testHost.StartAsync(cancellationToken);
			await kestrelHost.StartAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			await testHost.StopAsync(cancellationToken);
			await kestrelHost.StopAsync(cancellationToken);
		}
	}

	private class MessageSinkProvider : ILoggerProvider
	{
		private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
		private IMessageSink? output;

		public MessageSinkProvider(IMessageSink output)
		{
			this.output = output;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return _loggers.GetOrAdd(categoryName,
				name => output is null ? NullLogger.Instance : new MessageSinkLogger(name, output));
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) { output = null; }

		private class MessageSinkLogger : ILogger
		{
			private readonly string _name;
			private readonly IMessageSink _output;

			public MessageSinkLogger(string name, IMessageSink output)
			{
				_name = name;
				_output = output;
			}

			public IDisposable BeginScope<TState>(TState state) where TState : notnull
			{
				return default!;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return logLevel != LogLevel.None;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
				Func<TState, Exception?, string> formatter)
			{
				DiagnosticMessage message = new(_name + ":" + formatter(state, exception));
				_output.OnMessage(message);
			}
		}
	}

	#region "Overridable Properties"

	// Properties in this region can be overridden in a derived type and used as a fixture
	// If you create multiple derived fixtures, and derived tests injecting each one into a base test class
	// you can easily setup a test matrix for running a set of tests against multiple browsers and/or environments
	public virtual string? Environment { get; }
	public virtual PlaywrightBrowserType BrowserType => PlaywrightBrowserType.Chromium;

	protected virtual BrowserTypeLaunchOptions LaunchOptions { get; } = new() { Headless = true };

	public virtual bool AddMessageSinkProvider => true;
	public virtual LogLevel MinimumLogLevel => LogLevel.Trace;

	#endregion
}