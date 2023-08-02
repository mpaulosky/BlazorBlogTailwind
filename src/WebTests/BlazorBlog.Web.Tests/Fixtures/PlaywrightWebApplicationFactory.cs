// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     PlaywrightWebApplicationFactory.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

namespace BlazorBlog.Web.WebTests.Fixtures;

/// <summary>
///   WebApplicationFactory that wraps the TestHost in a Kestrel server.
///   <p>
///     Credit to <a href="https://github.com/CZEMacLeod">https://github.com/CZEMacLeod</a> for writing this.
///   </p>
/// </summary>
[ExcludeFromCodeCoverage]
public class PlaywrightWebApplicationFactory : WebApplicationFactory<AssemblyClassLocator>, IAsyncLifetime
{
	private IPlaywright? _playwright;
	private IBrowser? _browser;
	private string? _uri;
	private readonly IMessageSink _output;

	private static int _nextPort = 0;
	private bool _headless = true;

	public string? Uri => _uri;

	protected virtual string? Environment { get; } = "Development";

	public PlaywrightWebApplicationFactory(IMessageSink output) => this._output = output;

	[MemberNotNull(nameof(_uri))]
	protected override IHost CreateHost(IHostBuilder builder)
	{
		if (Environment is not null)
		{
			builder.UseEnvironment(Environment);
		}

		builder.ConfigureLogging(logging =>
		{
			logging.SetMinimumLevel(LogLevel.Trace);
			logging.AddProvider(new MessageSinkProvider(_output));
		});

		// We randomize the server port so we ensure that any hard coded Uri's fail in the tests.
		// This also allows multiple servers to run during the tests.
		var port = 5000 + Interlocked.Add(ref _nextPort, 10 + System.Random.Shared.Next(10));
		_uri = $"http://localhost:{port}";

		// We the testHost, which can be used with HttpClient with a custom transport
		// It is assumed that the return of CreateHost is a host based on the TestHost Server.
		var testHost = base.CreateHost(builder);

		// Now we reconfigure the builder to use kestrel so we have an http listener that can be used by playwright
		builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel(options =>
		{
			options.ListenLocalhost(port);
		}));
		var host = base.CreateHost(builder);

		UpdateUriFromHost(host); // For some reason, the kestrel server host does not seem to return the addresses.

		return new CompositeHost(testHost, host);
	}

	private void UpdateUriFromHost(IHost host)
	{
		var server = host.Services.GetRequiredService<IServer>();
		var addresses = server.Features.Get<IServerAddressesFeature>() ??
		                throw new NullReferenceException("Could not get IServerAddressesFeature");
		var serverAddress = addresses.Addresses.FirstOrDefault();

		if (serverAddress is not null)
		{
			_uri = serverAddress;
		}
		else
		{
			var message = new Xunit.Sdk.DiagnosticMessage("Could not get server address from IServerAddressesFeature");
			_output.OnMessage(message);
		}
	}

	public async Task<IPage> CreatePlaywrightPageAsync(bool headless = true)
	{
		_headless = headless;

		var server = Server; // Ensure Server is initialized
		await InitializeAsync(); // Ensure Playwright is initialized

		return await _browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = _uri });
	}

	[MemberNotNull(nameof(_playwright), nameof(_browser))]
	public async Task InitializeAsync()
	{
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
		_playwright ??= (await Playwright.CreateAsync()) ?? throw new InvalidOperationException();
		_browser ??= (await _playwright.Chromium.LaunchAsync(new() { Headless = _headless, Devtools = true })) ??
		             throw new InvalidOperationException();
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

	// CompositeHost is based on https://github.com/xaviersolau/DevArticles/blob/e2e_test_blazor_with_playwright/MyBlazorApp/MyAppTests/WebTestingHostFactory.cs
	// Relay the call to both test host and kestrel host.
	public class CompositeHost : IHost
	{
		private readonly IHost _testHost;
		private readonly IHost _kestrelHost;

		public CompositeHost(IHost testHost, IHost kestrelHost)
		{
			this._testHost = testHost;
			this._kestrelHost = kestrelHost;
		}

		public IServiceProvider Services => _testHost.Services;

		public void Dispose()
		{
			_testHost.Dispose();
			_kestrelHost.Dispose();
		}

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			await _testHost.StartAsync(cancellationToken);
			await _kestrelHost.StartAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken = default)
		{
			await _testHost.StopAsync(cancellationToken);
			await _kestrelHost.StopAsync(cancellationToken);
		}
	}

	private class MessageSinkProvider : ILoggerProvider
	{
		private IMessageSink? _output;

		private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

		public MessageSinkProvider(IMessageSink output) => this._output = output;

		public ILogger CreateLogger(string categoryName) =>
			_loggers.GetOrAdd(categoryName,
				name => _output is null ? NullLogger.Instance : new MessageSinkLogger(name, _output));

		protected virtual void Dispose(bool disposing) { _output = null; }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private class MessageSinkLogger : ILogger
		{
			private readonly string _name;
			private readonly IMessageSink _output;

			public MessageSinkLogger(string name, IMessageSink output)
			{
				this._name = name;
				this._output = output;
			}

			public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

			public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
				Func<TState, Exception?, string> formatter)
			{
				var message = new Xunit.Sdk.DiagnosticMessage(_name + ":" + formatter(state, exception));
				_output.OnMessage(message);
			}
		}
	}
}