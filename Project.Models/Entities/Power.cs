using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
	public class Power
	{
		public string PowerId { get; set; }
		public string PowerName { get; set; }
		public string ParentId { get; set; }
		public string PowerType { get; set; }
		public string Icon { get; set; }
		public string Path { get; set; }
	}
}
