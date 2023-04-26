using Orbit.Models.OrbitDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Core.DataAccess
{
	public class CompanyService : OrganizationService<Company>, ICompanyService
	{
		public CompanyService(ISqlClient sqlClient) : base(sqlClient)
		{
		}
	}
}
