using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace Milky.Models
{
	public class Product
	{
		[Key] //primary key
		public int id { get; set; } 

		[Required] 

		[MaxLength(30)] 

		[DisplayName("Product Name")]
		public string ProductName { get; set; }

		public string Description { get; set; }


		[Range (1,100)]
		public double Price { get; set; }


		[DisplayName("Milk Fat Percentage")]
		public string MilkFat { get; set; }

		public int CategoryID { get; set; }
		[ForeignKey("CategoryID")]

		[ValidateNever]
		public Category Category { get; set; }

		[ValidateNever]
		public string ImageUrl { get; set; }

        [ValidateNever]
        [DisplayName("Is Tax Included")]
        public string TaxIncluded { get; set; }

		[ValidateNever]
		[DisplayName("Biological Source")]
        public string? BiologicalSource { get; set; }

        [ValidateNever]
		public string? Flavour { get; set;}

        [ValidateNever]
        [DisplayName("Item Form")]
        public string? ItemForm { get; set; }

		[ValidateNever]
        [DisplayName("Net Quantity")]
		public string? NetQuantity { get; set; }

		[ValidateNever]
        [DisplayName("Number of Items")]
        public uint? NumberOfItems { get; set; }

		[ValidateNever]
        [DisplayName("Diet Type")]
        public string? DietType { get; set; }

        [DisplayName("Is Item Currently in Stock?")]
        public string? isItemInStock { get; set; }

		[DisplayName("Total Number of Items Currently in Stock")]

		[Range(0, uint.MaxValue, ErrorMessage = "Value must be greater than -1.")]
		public uint MaxNumberOfItemsInStock { get; set; }






    }
}
