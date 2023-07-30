// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     BlogPostDto.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common
// =============================================

using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.Common;

public class BlogPostDto
{
	[Required]
	[StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
		MinimumLength = 1)]
	public string Url { get; set; } = "";

	[Required]
	[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
		MinimumLength = 1)]
	public string Title { get; set; } = "";

	[Required]
	[StringLength(200, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
		MinimumLength = 1)]
	public string Description { get; set; } = "";

	[Required]
	[StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
		MinimumLength = 1)]
	public string Content { get; set; } = "";

	[Required]
	[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
		MinimumLength = 1)]
	public string Author { get; set; } = "";

	[Required]
	[DataType(DataType.DateTime)]
	public DateTime Created { get; set; } = DateTime.Now;

	[Required]
	[Display(Name = "Published")]
	public bool IsPublished { get; set; } = true;

	public string Image { get; set; } = string.Empty;

	public bool IsDeleted { get; set; } = false;
}