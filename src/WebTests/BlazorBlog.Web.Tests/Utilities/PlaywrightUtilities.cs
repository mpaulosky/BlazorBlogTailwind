// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     PlaywrightUtilities.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

namespace BlazorBlog.Web.WebTests.Utilities;

[ExcludeFromCodeCoverage]
public class PlaywrightUtilities
{
	private static readonly object InstallLock = new();
	private static readonly List<PlaywrightBrowserType> Installed = new();

	/// <summary>
	///   Install and deploy all binaries Playwright may need.
	/// </summary>
	internal static void InstallPlaywright(PlaywrightBrowserType? browser = null)
	{
		// Lock here so we don't try installing from multiple fixtures.
		lock (InstallLock)
		{
			if (browser is null)
			{
				if (Installed.Contains(PlaywrightBrowserType.Chromium) &&
				    Installed.Contains(PlaywrightBrowserType.Firefox) &&
				    Installed.Contains(PlaywrightBrowserType.Webkit))
				{
					return;
				}
			}
			else if (Installed.Contains(browser.Value))
			{
				return;
			}

			List<string> parameters = new() { "install" };
			if (browser is not null)
			{
				parameters.Add(browser.Value.ToString().ToLowerInvariant());
			}

			parameters.Add("--with-deps");
			int exitCode = Program.Main(parameters.ToArray());
			if (exitCode != 0)
			{
				throw new Exception(
					$"Playwright exited with code {exitCode} on {string.Join(' ', parameters)}");
			}

			if (browser is null)
			{
				Installed.Add(PlaywrightBrowserType.Chromium);
				Installed.Add(PlaywrightBrowserType.Firefox);
				Installed.Add(PlaywrightBrowserType.Webkit);
			}
			else
			{
				Installed.Add(browser.Value);
			}
		}
	}

	internal static void Uninstall(PlaywrightBrowserType? browser = null)
	{
		lock (InstallLock)
		{
			int exitCode = Program.Main(
				new[] { "uninstall", browser is null ? "--all" : browser.Value.ToString().ToLowerInvariant() });
			if (exitCode != 0)
			{
				throw new Exception(
					$"Playwright exited with code {exitCode} on uninstall -all");
			}

			if (browser is null)
			{
				Installed.Clear();
			}
			else
			{
				Installed.Remove(browser.Value);
			}
		}
	}

	public static void ShowTrace(string traceFile, params string[] options)
	{
		List<string> args = new() { "show-trace" };
		args.AddRange(options);
		args.Add(traceFile);
		int exitCode = Program.Main(args.ToArray());
		if (exitCode != 0)
		{
			throw new Exception(
				$"Playwright exited with code {exitCode} on show-trace");
		}
	}

	public static Task ShowTraceAsync(string traceFile, params string[] options)
	{
		return Task.Run(() =>
		{
			List<string> args = new() { "show-trace" };
			args.AddRange(options);
			args.Add(traceFile);
			int exitCode = Program.Main(args.ToArray());
			if (exitCode != 0)
			{
				throw new Exception(
					$"Playwright exited with code {exitCode} on show-trace");
			}
		});
	}
}