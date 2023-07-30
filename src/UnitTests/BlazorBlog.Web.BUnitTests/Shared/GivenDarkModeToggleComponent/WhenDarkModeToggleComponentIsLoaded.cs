// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     WhenDarkModeToggleComponentIsLoaded.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.BUnitTests
// =============================================

namespace BlazorBlog.Web.BUnitTests.Shared.GivenDarkModeToggleComponent;

[ExcludeFromCodeCoverage]
public class WhenDarkModeToggleComponentIsLoaded : TestContext
{
	private readonly Mock<ILocalStorageService> _localStorageMock;

	public WhenDarkModeToggleComponentIsLoaded()
	{
		_localStorageMock = new Mock<ILocalStorageService>();
	}

	[Fact]
	public void ShouldDisplayComponent_Test()
	{
		// Arrange
		SetupMocks();
		RegisterServices();

		IRenderedComponent<DarkModeToggle> cut = RenderComponent<DarkModeToggle>();

		// Act

		// Assert
		cut.MarkupMatches
		(
			"""
			<div title="Dark Mode Toggle" class="flex justify-end items-center space-x-2">
				<span class="text-gray-800 dark:text-gray-500">
					Light
				</span>
				<label for="toggle" class="w-9 h-5 flex items-center bg-gray-300 rounded-full p-1 cursor-pointer duration-300 ease-in-out dark:bg-gray-600">
					<div class="w-4 h-4 rounded-full bg-white shadow-md transform duration-300 ease-in-out dark:translate-x-3"></div>
				</label>
				<span class="text-gray-400 dark:text-white">
					Dark
				</span>
				<input id="toggle" type="checkbox" class="hidden">
			</div>
			"""
		);
	}

	private void SetupMocks()
	{
		BunitJSModuleInterop moduleInterop = JSInterop.SetupModule("./Shared/DarkModeToggle.razor.js");
		moduleInterop.Mode = JSRuntimeMode.Loose;
	}

	private void SetAuthenticationAndAuthorization(bool isAdmin, bool isAuth)
	{
		//TestAuthorizationContext authContext = this.AddTestAuthorization();

		//if (isAuth)
		//{
		//	authContext.SetAuthorized(_expectedUser!.DisplayName);
		//	authContext.SetClaims(
		//		new Claim("objectidentifier", _expectedUser.Id)
		//	);
		//}

		//if (isAdmin)
		//{
		//	authContext.SetPolicies("Admin");
		//}
	}

	private void RegisterServices()
	{
		Services.AddSingleton(_localStorageMock.Object);
	}

	private void SetMemoryCache()
	{
	}
}