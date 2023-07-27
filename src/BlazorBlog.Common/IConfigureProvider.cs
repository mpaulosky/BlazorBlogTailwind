using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlog.Common;

public interface IConfigureProvider
{

	IServiceCollection RegisterServices(IServiceCollection services, IConfiguration configuration);

}