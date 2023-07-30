// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     BlogPost.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common
// =============================================

namespace BlazorBlog.Common;

public class BlogPost
{
	public BlogPost() { }

	public BlogPost(BlogPostDto source)
	{
		Url = source.Url;
		Title = source.Title;
		Content = source.Content;
		Author = source.Author;
		Description = source.Description;
		Image = source.Image;
		IsDeleted = source.IsDeleted;
		Created = source.Created;
	}

	public int Id { get; set; }
	public string Url { get; set; } = "";
	public string Title { get; set; } = "";
	public string Content { get; set; } = "";
	public DateTime Created { get; set; } = DateTime.Now;
	public DateTime? Updated { get; set; }
	public string Author { get; set; } = "";
	public string Description { get; set; } = "";
	public string? Image { get; set; } = "";
	public bool IsPublished { get; set; } = true;
	public bool IsDeleted { get; set; }
}