using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IUserService
	{
		Task<User> AddUser(User user);
		Task<User?> GetUserByEmail(string email);
		Task<User?> GetUserByID(int id);
		Task<List<UserRole>> GetUserRoles(int userId);
		Task<List<Role>> GetAllRoles();
		Task<UserRole> AddRole(UserRole userRole);
		Task DeleteRole(UserRole userRole);
	}
}