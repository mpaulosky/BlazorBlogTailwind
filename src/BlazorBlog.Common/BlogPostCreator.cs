// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     BlogPostCreator.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common
// =============================================

using Bogus;

namespace BlazorBlog.Common;

public static class BlogPostCreator
{
	/// <summary>
	///   Gets a new post.
	/// </summary>
	/// <param name="keepId">bool whether to keep the generated Id</param>
	/// <param name="useNewSeed">bool whether to use a seed other than 0</param>
	/// <returns>BlogPost</returns>
	public static BlogPost GetNewBlogPost(bool keepId = false, bool useNewSeed = false)
	{
		BlogPost? post = GenerateFake(useNewSeed).Generate();

		if (!keepId)
		{
			post.Id = 0;
		}

		return post;
	}

	/// <summary>
	///   Gets a list of posts.
	/// </summary>
	/// <param name="numberOfPosts">The number of posts.</param>
	/// <param name="useNewSeed">bool whether to use a seed other than 0</param>
	/// <returns>A List of BlogPost</returns>
	public static List<BlogPost> GetBlogPosts(int numberOfPosts, bool useNewSeed = false)
	{
		List<BlogPost>? posts = GenerateFake(useNewSeed).Generate(numberOfPosts);

		return posts;
	}

	/// <summary>
	///   GenerateFake method
	/// </summary>
	/// <param name="useNewSeed">bool whether to use a seed other than 0</param>
	/// <returns>Fake BlogPost</returns>
	private static Faker<BlogPost> GenerateFake(bool useNewSeed = false)
	{
		int seed = 0;
		if (useNewSeed)
		{
			seed = Random.Shared.Next(10, int.MaxValue);
		}

		return new Faker<BlogPost>()
			.RuleFor(x => x.Id, f => f.Random.Int(0, 100))
			.RuleFor(x => x.Url, f => $"{f.Name.FirstName()}-{f.Name.LastName()}")
			.RuleFor(c => c.Title, f => f.Lorem.Sentence(10))
			.RuleFor(x => x.Description, f => f.Lorem.Paragraph(1))
			.RuleFor(x => x.Content, f => f.Lorem.Paragraphs(10))
			.RuleFor(x => x.Author, f => f.Name.FullName())
			.RuleFor(x => x.Created, f => f.Date.Past())
			.RuleFor(x => x.Updated, f => f.Date.Past())
			.RuleFor(x => x.IsPublished, f => f.Random.Bool())
			.RuleFor(x => x.IsDeleted, f => f.Random.Bool())
			.RuleFor(x => x.Image, f => f.Image.PicsumUrl(2500, 1667, false, false, 12))
			.UseSeed(seed);
	}
}