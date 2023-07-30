// ============================================
// Copyright (c) 2023. All rights reserved.
// File Name :     ServicesExtensions.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : BlazorBlogTailwind
// Project Name :  BlazorBlog.Web
// =============================================

public static class ServicesExtensions
{
	//public static IServiceCollection AddTagzAppHostedServices(this IServiceCollection services, IConfigurationRoot configuration)
	//{

	//	services.AddSingleton<InMemoryMessagingService>();
	//	services.AddHostedService(s => s.GetRequiredService<InMemoryMessagingService>());

	//	// Register the providers
	//	if (SocialMediaProviders.Any())
	//	{
	//		foreach (var item in SocialMediaProviders)
	//		{
	//			services.ConfigureProvider(item, configuration);
	//		}
	//	}
	//	else
	//	{
	//		services.ConfigureProvider<StartMastodon>(configuration);
	//	}

	//	return services;

	//}

	/// <summary>
	///   A collection of externally configured providers
	/// </summary>
	public static List<IConfigureProvider> SocialMediaProviders { get; set; } = new();

	public static IServiceCollection ConfigureProvider<T>(this IServiceCollection services, IConfiguration configuration)
		where T : IConfigureProvider, new()
	{
		IConfigureProvider providerStart = (IConfigureProvider)Activator.CreateInstance<T>();
		providerStart.RegisterServices(services, configuration);

		return services;
	}

	public static IServiceCollection ConfigureProvider(this IServiceCollection services, IConfigureProvider provider,
		IConfiguration configuration)
	{
		provider.RegisterServices(services, configuration);

		return services;
	}
}