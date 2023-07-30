// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     IConfigureProvider.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Common
// =============================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlog.Common;

public interface IConfigureProvider
{
	IServiceCollection RegisterServices(IServiceCollection services, IConfiguration configuration);
}