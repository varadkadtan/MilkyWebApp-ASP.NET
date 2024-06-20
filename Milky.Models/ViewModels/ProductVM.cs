using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.Models.ViewModels
{
	public class ProductVM
	{
        public Product Product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        // New property to store selected category IDs
        [ValidateNever]
        public List<int> SelectedCategoryIds { get; set; }

        [ValidateNever]
        public IEnumerable<Product> ProductList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> IsTaxIncludedOptions { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> biologicalSourceOptions { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> flavorOptions{ get; set; }

        [ValidateNever]
		public IEnumerable<SelectListItem> UnitOptions { get; set; } //for netQuantity

		[ValidateNever]
		public string? SelectedUnit { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ItemFormOptions { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> DietTypeOptions { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> isItemInStockOptions { get; set; }  
	}
}
