using Microsoft.Extensions.DependencyInjection;
using Orbit.Core.DataAccess;

namespace Orbit.Core.Extensions
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddOrbitCore(this IServiceCollection services)
		{
			services.AddTransient<IUserSession, UserSession>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<IUserRoleService, UserRoleService>();
			services.AddTransient<IOrganizationService, OrganizationService>();
			services.AddTransient<ISqlClient, SqlClient>();

			return services;
		}
	}
}
