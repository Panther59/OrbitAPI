using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public class UserRoleService : IUserRoleService
	{
		private readonly ISqlClient sqlClient;

		public UserRoleService(ISqlClient sqlClient)
		{
			this.sqlClient = sqlClient;
		}

		public async Task<List<UserRole>> GetUserRoles(int userId)
		{
			string sql = $"EXEC spGetUserRoles '{userId}'";
			return await this.sqlClient.GetData<UserRole>(sql);
		}

		public async Task<List<Role>> GetAllRoles()
		{
			return await this.sqlClient.GetAllData<Role>();
		}

		public async Task<UserRole> AddRole(UserRole userRole)
		{
			string sql = $"SELECT * FROM tblUserRoles (NOLOCK) where UserID = '{userRole.UserID}' and RoleID = '{userRole.RoleID}'";
			if (userRole.CompanyID.HasValue)
			{
				sql += $" and CompanyID = '{userRole.CompanyID}'";
			}
			else if (userRole.ClientID.HasValue)
			{
				sql += $" and ClientID = '{userRole.ClientID}'";
			}

			var results = await this.sqlClient.GetData<UserRole>(sql);
			var existingUserRole = results?.FirstOrDefault();
			if (existingUserRole == null)
			{
				await this.sqlClient.InsertAsync(userRole);

				return userRole;
			}
			else
			{
				throw new Exception($"This permission already exists");
			}
		}

		public async Task DeleteRole(UserRole userRole)
		{
			string sql = $"DELETE FROM tblUserRoles where UserID = '{userRole.UserID}' and RoleID = '{userRole.RoleID}'";
			if (userRole.CompanyID.HasValue)
			{
				sql += $" and CompanyID = '{userRole.CompanyID}'";
			}
			else if (userRole.ClientID.HasValue)
			{
				sql += $" and ClientID = '{userRole.ClientID}'";
			}

			await this.sqlClient.ExecuteAsync(sql);
		}
	}
}
