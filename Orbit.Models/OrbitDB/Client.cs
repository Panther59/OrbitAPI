using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Dapper.Contrib.Extensions.Table("tblClients")]
	public class Client : Organization
	{
	}
}
