using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IUserRoleService
	{
		Task<UserRole> AddRole(UserRole userRole);
		Task DeleteRole(UserRole userRole);
		Task<List<Role>> GetAllRoles();
		Task<List<UserRole>> GetUserRoles(int? userId = null, int? organizationId = null);
		Task<List<Organization>> GetUserOrganizations(int userId);
		Task<List<string>> GetUserPermissions(int userId, int? organizationId = null);
	}
}