// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     WhenRequiringABlogPost.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common.UnitTests
// =============================================

namespace BlazorBlog.Common.UnitTests.GivenBlogPostDto;

[ExcludeFromCodeCoverage]
public class WhenRequiringABlogPost
{
	[Fact]
	public void ShouldConvertBlogPostDtoToBlogPost_Test()
	{
		// Arrange
		BlogPostDto expectedDto = BlogPostDtoCreator.GetNewBlogPostDto()!;

		// Act
		BlogPost result = new(expectedDto);

		// Assert
		result.Should().BeEquivalentTo(expectedDto,
			options => options
				.Excluding(t => t.Created)
				.Excluding(t => t.IsPublished));
	}

	[Fact]
	public void ShouldConvertBlogPostDtoToBlogPostWithNewSeed_Test()
	{
		// Arrange
		BlogPostDto expectedDto = BlogPostDtoCreator.GetNewBlogPostDto(true)!;

		// Act
		BlogPost result = new(expectedDto);

		// Assert
		result.Should().BeEquivalentTo(expectedDto,
			options => options
				.Excluding(t => t.Created)
				.Excluding(t => t.IsPublished));
	}
}