using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.Models.ViewModels
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
		public OrderHeader OrderHeader { get; set; }
		//public double OrderTotal { get; set; }
	}
}
