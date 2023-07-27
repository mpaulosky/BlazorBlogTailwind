using BlazorBlog.Web.Tests.Fixtures;

namespace BlazorBlog.Web.Tests.Utilities;

public class PlaywrightUtilities
{

	private static readonly object InstallLock = new();
	private static readonly List<PlaywrightBrowserType> Installed = new();
	/// <summary>
	/// Install and deploy all binaries Playwright may need.
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
			var parameters = new List<string>()
						{
								"install"
						};
			if (browser is not null)
			{
				parameters.Add(browser.Value.ToString().ToLowerInvariant());
			}
			parameters.Add("--with-deps");
			var exitCode = Program.Main(parameters.ToArray());
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
			var exitCode = Program.Main(
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
		var args = new List<string>() { "show-trace" };
		args.AddRange(options);
		args.Add(traceFile);
		var exitCode = Program.Main(args.ToArray());
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
			var args = new List<string>() { "show-trace" };
			args.AddRange(options);
			args.Add(traceFile);
			var exitCode = Program.Main(args.ToArray());
			if (exitCode != 0)
			{
				throw new Exception(
							$"Playwright exited with code {exitCode} on show-trace");
			}
		});
	}
}
