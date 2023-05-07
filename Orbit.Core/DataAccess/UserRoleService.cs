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

		public async Task<List<UserRole>> GetUserRoles(int? userId = null, int? companyId = null, int? clientId = null)
		{
			string sql = $"EXEC [dbo].[spGetUserRoles] " +
				$"@userId = {(userId.HasValue ? userId.Value : "NULL")}, " +
				$"@companyId = {(companyId.HasValue ? companyId.Value : "NULL")}, " +
				$"@clientId = {(clientId.HasValue ? clientId.Value : "NULL")} ";

			return await this.sqlClient.GetData<UserRole>(sql);
		}

		public async Task<List<Role>> GetAllRoles()
		{
			return await this.sqlClient.GetAllData<Role>();
		}

		public async Task<UserRole> AddRole(UserRole userRole)
		{
			var results = await this.GetUserRoles(userRole.UserID, userRole.CompanyID, userRole.ClientID);
			var existingUserRole = results?.FirstOrDefault();
			if (existingUserRole != null)
			{
				if (existingUserRole.RoleID == userRole.RoleID)
				{
					throw new Exception($"This permission already exists for {existingUserRole.User}");
				}
				else if (existingUserRole.RoleID < userRole.RoleID)
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
