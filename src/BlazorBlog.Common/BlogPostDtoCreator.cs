// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     BlogPostDtoCreator.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common
// =============================================

using Bogus;

namespace BlazorBlog.Common;

public static class BlogPostDtoCreator
{
	/// <summary>
	///   Gets a new post dto.
	/// </summary>
	/// <param name="useNewSeed">bool whether to use a seed other than 0</param>
	/// <returns>BlogPostDto</returns>
	public static BlogPostDto GetNewBlogPostDto(bool useNewSeed = false)
	{
		BlogPostDto? post = GenerateFake(useNewSeed).Generate();

		return post;
	}

	/// <summary>
	///   GenerateFake method
	/// </summary>
	/// <param name="useNewSeed">bool whether to use a seed other than 0</param>
	/// <returns>Fake BlogPostDto</returns>
	private static Faker<BlogPostDto> GenerateFake(bool useNewSeed = false)
	{
		int seed = 0;
		if (useNewSeed)
		{
			seed = Random.Shared.Next(10, int.MaxValue);
		}

		return new Faker<BlogPostDto>()
			.RuleFor(x => x.Url, f => $"{f.Name.FirstName()}-{f.Name.LastName()}")
			.RuleFor(c => c.Title, f => f.Lorem.Sentence(10))
			.RuleFor(x => x.Description, f => f.Lorem.Paragraph(1))
			.RuleFor(x => x.Content, f => f.Lorem.Paragraphs(10))
			.RuleFor(x => x.Author, f => f.Name.FullName())
			.RuleFor(x => x.Created, f => f.Date.Past())
			.RuleFor(x => x.IsPublished, f => f.Random.Bool())
			.RuleFor(x => x.IsDeleted, f => f.Random.Bool())
			.RuleFor(x => x.Image, f => f.Image.PicsumUrl(2500, 1667, false, false, 12))
			.UseSeed(seed);
	}
}