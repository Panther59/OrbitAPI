using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.DTOs
{
	public class LoginWithOrganizationRequest
	{
		public string OrganizationType { get; set; }
		public string Organization { get; set; }
	}
}
