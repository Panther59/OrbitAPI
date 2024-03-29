﻿using Orbit.Models.OrbitDB;

namespace Orbit.Core.DataAccess
{
	public interface IUserService
	{
		Task<List<User>> GetAllUsers();
		Task<User> AddUser(User user);
		Task<User?> GetUserByEmail(string email);
		Task<User?> GetUserByID(int id);
	}
}