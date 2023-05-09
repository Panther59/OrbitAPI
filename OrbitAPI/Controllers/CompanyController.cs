using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.Core;
using Orbit.Core.DataAccess;
using Orbit.Models.OrbitDB;

namespace OrbitAPI.Controllers
{
	[Route("api/[controller]")]
	public class CompanyController : OrganizationController<Company>
	{
		public CompanyController(IUserSession userSession, ICompanyService companyService) : base(userSession, companyService)
		{
		}

		public override string OrgUpdatePermissionName => Permissions.ManageMyCompanyPermissions;

		public override string OrgUpdateAdminPermissionName => Permissions.ManageCompanies;
	}
}
