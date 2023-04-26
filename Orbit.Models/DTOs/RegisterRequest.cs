using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.DTOs
{
	public class RegisterRequest
	{
		public string? Email { get; set; }
		public string? GoogleToken { get; set; }
	}
}
