// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     PlaywrightTrace.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

namespace BlazorBlog.Web.WebTests.Utilities;

[ExcludeFromCodeCoverage]
public class PlaywrightTrace : IAsyncDisposable
{
	private readonly IPage _page;

	internal PlaywrightTrace(IPage page)
	{
		_page = page;
	}

	public string? TraceName { get; private set; }

	public async ValueTask DisposeAsync()
	{
		await _page.Context.Tracing.StopAsync(new TracingStopOptions { Path = TraceName });
	}

	internal async Task InitializeAsync(TracingStartOptions options)
	{
		await _page.Context.Tracing.StartAsync(options);
		TraceName = options.Name;
	}
}