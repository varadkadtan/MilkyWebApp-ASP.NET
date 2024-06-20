using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? Street { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Postal Code must be exactly 6 characters.")]
        public string? PostalCode { get; set; }


    }
}
