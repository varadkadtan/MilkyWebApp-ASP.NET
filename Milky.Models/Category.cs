using System.ComponentModel;
using System.ComponentModel.DataAnnotations; // For data annotations for validation and database schema generation

namespace Milky.Models
{
    public class Category
    {
        [Key] //primary key
        public int id { get; set; } 

        [Required] 
        [MaxLength(30)] //validations
        [DisplayName("Category Name")]
		public string name { get; set; }


		[DisplayName("Display Order")]
        [Range(1,100)] //validations
        public int displayOrder { get; set; }



    }
}
