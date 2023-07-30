// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     PlaywrightPageExtensions.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

namespace BlazorBlog.Web.WebTests.Utilities;

[ExcludeFromCodeCoverage]
public static class PlaywrightPageExtensions
{
	public static async Task<PlaywrightTrace> TraceAsync(this IPage page,
		string title, bool? screenshots = null, bool? snapshots = null, bool? sources = null,
		string? prefix = null,
		[CallerMemberName] string? name = null)
	{
		PlaywrightTrace trace = new(page);
		await trace.InitializeAsync(new TracingStartOptions
		{
			Screenshots = screenshots,
			Snapshots = snapshots,
			Sources = sources,
			Name = prefix is null ? $"{name}.zip" : $"{prefix}_{name}.zip",
			Title = title
		});
		return trace;
	}

	public static Task<PlaywrightTrace> TraceAsync<T>(this IPage page, string title, bool? screenshots = null,
		bool? snapshots = null, bool? sources = null, [CallerMemberName] string? name = null)
	{
		return page.TraceAsync(title, typeof(T), screenshots, snapshots, sources, name);
	}

	public static Task<PlaywrightTrace> TraceAsync(this IPage page, string title, Type type, bool? screenshots = null,
		bool? snapshots = null, bool? sources = null, [CallerMemberName] string? name = null)
	{
		return page.TraceAsync(title, screenshots, snapshots, sources, type.FullName, name);
	}

	public static Task<PlaywrightTrace> TraceAsync<T>(this IPage page, string title, T prefix, bool? screenshots = null,
		bool? snapshots = null, bool? sources = null, [CallerMemberName] string? name = null)
		where T : class
	{
		return page.TraceAsync(title, screenshots, snapshots, sources, prefix.ToString(), name);
	}
}