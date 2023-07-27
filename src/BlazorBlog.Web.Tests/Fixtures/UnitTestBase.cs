namespace BlazorBlog.Web.Tests.Fixtures;

public abstract class UnitTestBase
{
	protected readonly PlaywrightWebApplicationFactory<AssemblyClassLocator> _webApplication;
	protected readonly ITestOutputHelper _outputHelper;

	public UnitTestBase(PlaywrightWebApplicationFactory<AssemblyClassLocator> webApplication, ITestOutputHelper outputHelper)
	{
		_webApplication = webApplication;
		_outputHelper = outputHelper;
	}
}