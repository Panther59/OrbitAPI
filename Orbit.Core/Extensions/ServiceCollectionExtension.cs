using Microsoft.Extensions.DependencyInjection;
using Orbit.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Core.Extensions
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddOrbitCore(this IServiceCollection services)
		{
			services.AddTransient<IUserSession, UserSession>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<ISqlClient, SqlClient>();

			return services;
		}
	}
}
