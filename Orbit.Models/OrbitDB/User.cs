﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.Models.OrbitDB
{
	[Dapper.Contrib.Extensions.Table("tblUsers")]
	public class User
	{
		[ExplicitKey]
		public int ID { get; set; }
		public string Name => $"{FirstName} {LastName}";
		public string Initial => $"{FirstName?.Substring(0,1)}{LastName.Substring(0, 1)}";
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public DateTime Created { get; set; }
		public int CreatedBy { get; set; }
		public DateTime? Updated { get; set; }
		public int? UpdatedBy { get; set; }
		public string? Picture { get; set; }
	}
}
