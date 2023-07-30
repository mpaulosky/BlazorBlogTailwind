﻿// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     IndexTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

namespace BlazorBlog.Web.WebTests.Pages;

[ExcludeFromCodeCoverage]
public class IndexTests : UnitTestBase, IClassFixture<PlaywrightWebApplicationFactory<AssemblyClassLocator>>
{
	public IndexTests(PlaywrightWebApplicationFactory<AssemblyClassLocator> webApplication,
		ITestOutputHelper outputHelper) :
		base(webApplication, outputHelper)
	{
	}

	[Fact]
	public async Task CanSaveTrace()
	{
		const string expectedZipFileName = $"{nameof(CanSaveTrace)}.zip";
		const string expectedPageTitle = "Home";

		IPage page = await _webApplication.CreatePlaywrightPageAsync();
		await using PlaywrightTrace trace = await page.TraceAsync(
			$"Testing Tracing on {_webApplication.BrowserType}",
			true,
			true,
			true);

		_outputHelper.WriteLine($"Tracing to {trace.TraceName}");

		await page.GotoAsync("/");

		await page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Hello, world!" })
			.WaitForAsync(new LocatorWaitForOptions { Timeout = 2500 });

		trace.TraceName.Should().Be(expectedZipFileName);
		string title = await page.TitleAsync();
		title.Should().Be(expectedPageTitle);
	}

	[Fact]
	public async Task TestRootTitle()
	{
		const string expectedPageTitle = "Home";

		IPage page = await _webApplication.CreatePlaywrightPageAsync();

		await page.GotoAsync("/");

		await page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Hello, world!" })
			.WaitForAsync(new LocatorWaitForOptions { Timeout = 2500 });

		bool header = await page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Hello, world!" })
			.IsVisibleAsync();
		bool message = await page.GetByText("Welcome to your new app.").IsVisibleAsync();
		string title = await page.TitleAsync();

		title.Should().Be(expectedPageTitle);
		header.Should().BeTrue();
		message.Should().BeTrue();
	}

	[Fact]
	public async Task Test404()
	{
		const string expectedPageTitle = "Not found";

		IPage page = await _webApplication.CreatePlaywrightPageAsync();

		await page.GotoAsync("/Unknown");

		await page.GetByText("Sorry, there's nothing at this address.")
			.WaitForAsync(new LocatorWaitForOptions { Timeout = 2500 });

		bool message = await page.GetByText("Sorry, there's nothing at this address.").IsVisibleAsync();
		string title = await page.TitleAsync();

		title.Should().Be(expectedPageTitle);
		message.Should().BeTrue();
	}

	[Fact]
	public async Task TestError()
	{
		const string expectedPageTitle = "Not found";

		IPage page = await _webApplication.CreatePlaywrightPageAsync();

		_ = await page.GotoAsync("/BadRequest");

		await page.GetByText("Sorry, there's nothing at this address.")
			.WaitForAsync(new LocatorWaitForOptions { Timeout = 2500 });

		IHostEnvironment hostEnv = _webApplication.Server.Services.GetRequiredService<IHostEnvironment>();
		_outputHelper.WriteLine("Host Environment {0}", hostEnv.EnvironmentName);

		if (hostEnv.IsDevelopment())
		{
			// The title of the page from DeveloperExceptionPageMiddleware
			(await page.TitleAsync()).Should().Be(expectedPageTitle);
		}
		else
		{
			// The title of the page for Error.cshtml
			(await page.TitleAsync()).Should().Be(expectedPageTitle);
		}
	}
}