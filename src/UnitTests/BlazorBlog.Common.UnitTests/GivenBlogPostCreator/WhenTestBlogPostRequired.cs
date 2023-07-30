// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     WhenTestBlogPostRequired.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common.UnitTests
// =============================================

namespace BlazorBlog.Common.UnitTests.GivenBlogPostCreator;

[ExcludeFromCodeCoverage]
public class WhenTestBlogPostRequired
{
	[Fact]
	public void ShouldReturnNewBlogPostWithoutId_Test()
	{
		// Arrange
		BlogPost expected = BlogPostCreator.GetNewBlogPost()!;

		// Act
		BlogPost result = BlogPostCreator.GetNewBlogPost();

		// Assert
		result.Should().BeEquivalentTo(expected,
			options => options
				.Excluding(t => t.Created)
				.Excluding(t => t.Updated));
	}

	[Fact]
	public void ShouldReturnNewBlogPostWithId_Test()
	{
		// Arrange
		BlogPost expected = BlogPostCreator.GetNewBlogPost(true)!;

		// Act
		BlogPost result = BlogPostCreator.GetNewBlogPost(true);

		// Assert
		result.Should().BeEquivalentTo(expected,
			options => options
				.Excluding(t => t.Created)
				.Excluding(t => t.Updated));
	}

	[Fact]
	public void ShouldReturnDifferentNewBlogPostWhenNewSeedIsTrue_Test()
	{
		// Arrange
		BlogPost expected = BlogPostCreator.GetNewBlogPost(true)!;

		// Act
		BlogPost result = BlogPostCreator.GetNewBlogPost(true, true);

		// Assert
		result.Should().NotBeEquivalentTo(expected);
	}

	[Fact]
	public void ShouldReturnAListOfBlogPosts_Test()
	{
		// Arrange
		List<BlogPost> expected = BlogPostCreator.GetBlogPosts(3)!;

		// Act
		List<BlogPost> result = BlogPostCreator.GetBlogPosts(3);

		// Assert
		result.Should().BeEquivalentTo(expected,
			options => options
				.Excluding(t => t.Created)
				.Excluding(t => t.Updated));
	}
}