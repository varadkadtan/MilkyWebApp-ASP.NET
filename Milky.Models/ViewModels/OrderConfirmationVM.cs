using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milky.Models.ViewModels;
using Milky.Models;

namespace Milky.Models.ViewModels
{
	public class OrderConfirmationVM
	{
		public string? UniqueCodee { get; set; }

		public int? OrderId { get; set; }
	}
}
