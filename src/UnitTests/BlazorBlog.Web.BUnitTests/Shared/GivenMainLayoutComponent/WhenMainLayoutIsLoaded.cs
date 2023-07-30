// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     WhenMainLayoutIsLoaded.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.BUnitTests
// =============================================

namespace BlazorBlog.Web.BUnitTests.Shared.GivenMainLayoutComponent;

[ExcludeFromCodeCoverage]
public class WhenMainLayoutIsLoaded : TestContext
{
	private readonly Mock<ILocalStorageService> _localStorageMock;

	public WhenMainLayoutIsLoaded()
	{
		_localStorageMock = new Mock<ILocalStorageService>();
	}

	[Fact]
	public void ShouldDisplayNavMenuAndBody_Test()
	{
		// Arrange
		const int expectedLinkCount = 4;
		const int expectedInputCount = 1;

		SetupMocks();
		RegisterServices();
		SetAuthenticationAndAuthorization();

		// Act
		IRenderedComponent<MainLayout> cut = RenderComponent<MainLayout>();

		// Assert
		cut.FindAll("a").Count.Should().Be(expectedLinkCount);
		cut.FindAll("input").Count.Should().Be(expectedInputCount);
	}

	private void SetupMocks()
	{
		BunitJSModuleInterop moduleInterop = JSInterop.SetupModule("./Shared/DarkModeToggle.razor.js");
		moduleInterop.Mode = JSRuntimeMode.Loose;
		JSInterop.Setup<String>("localStorage.getItem", _ => true);
	}

	private void SetAuthenticationAndAuthorization(bool isAdmin = false, bool isAuth = true)
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
		Services.AddBlazoredLocalStorage();
	}
}