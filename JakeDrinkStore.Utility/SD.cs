using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";

		// Pending Status is where Order is placed and didn't complete the payment
		public const string StatusPending = "Pending";
		// Approved Status is where Order is placed and complete the payment
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusCompleted = "Completed";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";

        // Pending Payment Status is where Order is placed and didn't complete the payment
        public const string PaymentStatusPending = "Pending";
        // Approved Payment Status is where Order is placed and complete the payment
        public const string PaymentStatusApproved = "Approved";
		// Delayed Payment Status is for Company Users where Payment can be made within 30 days 
		public const string PaymentStatusDelayedPayment = "Delayed Payment";
		public const string PaymentStatusRejected = "Rejected";
		public const string SessionCart = "SessionShoppingCart";
	}
}
