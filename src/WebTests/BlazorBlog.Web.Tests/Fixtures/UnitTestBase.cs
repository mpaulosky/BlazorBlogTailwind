// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     UnitTestBase.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.WebTests
// =============================================

namespace BlazorBlog.Web.WebTests.Fixtures;

[ExcludeFromCodeCoverage]
public abstract class UnitTestBase
{
	protected readonly ITestOutputHelper _outputHelper;
	protected readonly PlaywrightWebApplicationFactory<AssemblyClassLocator> _webApplication;

	public UnitTestBase(PlaywrightWebApplicationFactory<AssemblyClassLocator> webApplication,
		ITestOutputHelper outputHelper)
	{
		_webApplication = webApplication;
		_outputHelper = outputHelper;
	}
}