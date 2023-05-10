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

		public async Task<List<UserRole>> GetUserRoles(int? userId = null, int? organizationId = null)
		{
			string sql = $"EXEC [dbo].[spGetUserRoles] " +
				$"@userId = {(userId.HasValue ? userId.Value : "NULL")}, " +
				$"@organizationId = {(organizationId.HasValue ? organizationId.Value : "NULL")}";

			return await this.sqlClient.GetData<UserRole>(sql);
		}

		public async Task<List<string>> GetUserPermissions(int userId, int? organizationId = null)
		{
			string sql = $"EXEC [dbo].[spGetUserPermissions] " +
				$"@userId = {userId}, " +
				$"@organizationId = {(organizationId.HasValue ? organizationId.Value : "NULL")}";

			return await this.sqlClient.GetData<string>(sql);
		}

		public async Task<List<Organization>> GetUserOrganizations(int userId)
		{
			var orgQuery = $"SELECT c.* FROM tblUserRoles (NOLOCK) ur INNER JOIN tblOrganizations (NOLOCK) c ON ur.OrganizationID = c.ID WHERE ur.UserID = {userId}";

			return await this.sqlClient.GetData<Organization>(orgQuery);
		}

		public async Task<bool> IsSuperUser(int userId)
		{
			var orgQuery = $"SELECT c.* FROM tblUserRoles (NOLOCK) ur WHERE ur.UserID = {userId} AND ur.OrganizationID is NULL";
			var orgs = await this.sqlClient.GetData<Organization>(orgQuery);
			return orgs.Any();
		}

		public async Task<List<Role>> GetAllRoles()
		{
			return await this.sqlClient.GetAllData<Role>();
		}

		public async Task<UserRole> AddRole(UserRole userRole)
		{
			var results = await this.GetUserRoles(userRole.UserID, userRole.OrganizationID);
			var existingUserRole = results?.FirstOrDefault();
			if (existingUserRole != null)
			{
				if (existingUserRole.RoleRanking == userRole.RoleRanking)
				{
					throw new Exception($"This permission already exists for {existingUserRole.User}");
				}
				else if (existingUserRole.RoleRanking < userRole.RoleRanking)
				{
					throw new Exception($"{existingUserRole.User} already has higher permission ({existingUserRole.Role}) available, if you want to change permission then edit existing permission");
				}
			}

			await this.sqlClient.InsertAsync(userRole);

			return userRole;
		}

		public async Task DeleteRole(UserRole userRole)
		{
			string sql = $"DELETE FROM tblUserRoles where UserID = '{userRole.UserID}' and RoleID = '{userRole.RoleID}'";
			if (userRole.OrganizationID.HasValue)
			{
				sql += $" and OrganizationID = '{userRole.OrganizationID}'";
			}

			await this.sqlClient.ExecuteAsync(sql);
		}


	}
}
