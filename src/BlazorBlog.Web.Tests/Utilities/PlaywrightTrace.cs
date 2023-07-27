namespace BlazorBlog.Web.Utilities;

public class PlaywrightTrace : IAsyncDisposable
{
	internal PlaywrightTrace(IPage page)
	{
		_page = page;
	}
	private readonly IPage _page;

	private string? _path;

	public string? TraceName => _path;

	internal async Task InitializeAsync(TracingStartOptions options)
	{
		await _page.Context.Tracing.StartAsync(options);
		_path = options.Name;
	}

	public async ValueTask DisposeAsync()
	{
		await _page.Context.Tracing.StopAsync(new()
		{
			Path = _path
		});
	}
}
