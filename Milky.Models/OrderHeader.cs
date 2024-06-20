using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		public DateTime OrderDate { get; set; }

		public bool IsOrderConfirmed { get; set; }

		public double OrderTotal { get; set; }

		public string? OrderStatus { get; set; }

		public string? PaymentStatus { get; set; }

		public DateTime PaymentDate { get; set; }

		public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		public string? UniqueCode { get; set; }

		public TimeOnly? StartedProcessingTime { get; set; }

		public TimeOnly? FinishedProcessingTime { get; set; }

        public TimeOnly? OrderPickupTime { get; set; }

    }
}
