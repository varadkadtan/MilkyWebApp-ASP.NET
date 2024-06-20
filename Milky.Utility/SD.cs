using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.Utility
{
	public static class SD
	{
		public const string Role_Customer = "Customer";
		public const string Role_Admin = "Admin";
		public const string Role_Company = "Company";
        public const string Role_Employee = "Employee";

		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusReadyforPickup = "Ready For Pickup";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";
		public const string StatusCompleted = "Completed";

		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStausRejected = "Rejected";

		//session related data

		public const string SessionCart = "SessionShoppingCart";

	}
}
