﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	public abstract class Organization : BaseAuditRecord
	{
		[ExplicitKey]
		public int ID { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string PinCode { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
	}
}