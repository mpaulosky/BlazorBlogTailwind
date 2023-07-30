// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     WhenDarkModeToggleIsClicked.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web.BUnitTests
// =============================================

using AngleSharp.Dom;

namespace BlazorBlog.Web.BUnitTests.Shared.GivenDarkModeToggleComponent;

[ExcludeFromCodeCoverage]
public class WhenDarkModeToggleIsClicked : TestContext
{
	private readonly Mock<ILocalStorageService> _localStorageMock;

	public WhenDarkModeToggleIsClicked()
	{
		_localStorageMock = new Mock<ILocalStorageService>();
	}

	[Theory]
	[InlineData(true, 5)]
	[InlineData(false, 4)]
	public void ShouldToggleTheMode(bool toggleValue, int expectedCount)
	{
		// Arrange
		SetupMocks();
		RegisterServices();

		IRenderedComponent<DarkModeToggle> cut = RenderComponent<DarkModeToggle>();

		// Act
		cut.Find("#toggle").Change(toggleValue);
		IElement result = cut.Find("#toggle");

		// Assert
		result.Attributes.Length.Should().Be(expectedCount);
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