using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.OrbitDB;

namespace OrbitAPI.Controllers
{
	[Route("api/[controller]")]
	public class ClientController : OrganizationController<Client>
	{
		public ClientController(IUserSession userSession, IClientService clientService) : base(userSession, clientService)
		{
		}
	}
}
