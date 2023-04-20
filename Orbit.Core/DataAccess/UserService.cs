using Orbit.Models.OrbitDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Core.DataAccess
{
	public class UserService : IUserService
	{
		private readonly ISqlClient sqlClient;

		public UserService(ISqlClient sqlClient)
		{
			this.sqlClient = sqlClient;
		}

		public async Task<User> AddUser(User user)
		{
			var existingUser = await this.GetUserByEmail(user.Email);
			if (existingUser == null)
			{
				string sql = $"SELECT MAX(ID) FROM tblUsers (NOLOCK)";
				var maxId = await this.sqlClient.GetMaxID("tblUsers");
				user.ID = (maxId ?? 0) + 1;
				await this.sqlClient.InsertAsync(user);

				return user;
			}
			else
			{
				if (existingUser.IsActive)
				{
					throw new Exception($"There is already a valid user in the system with email {user.Email}");
				}
				else
				{
					throw new Exception($"There is already a inactive user in the system with email {user.Email}, please reach out to admin to enable your account");
				}				
			}
		}

		public async Task<User?> GetUserByEmail(string email)
		{
			string sql = $"SELECT * FROM tblUsers (NOLOCK) where Email = '{email}'";
			var results = await this.sqlClient.GetData<User>(sql);
			return results?.FirstOrDefault();
		}

		public async Task<User?> GetUserByID(int id)
		{
			string sql = $"SELECT * FROM tblUsers (NOLOCK) where ID = '{id}'";
			var results = await this.sqlClient.GetData<User>(sql);
			return results?.FirstOrDefault();
		}
	}
}
