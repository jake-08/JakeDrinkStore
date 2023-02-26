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
		public const string OrderStatusPending = "Pending";
		// Approved Status is where Order is placed and complete the payment
		public const string OrderStatusApproved = "Approved";
		// Processing when Admin User Start Processing
		public const string OrderStatusInProcess = "Processing";
		// Completed when the order is shipped
		public const string OrderStatusCompleted = "Completed";
		// Cancel the order before shipping
		public const string OrderStatusCancelled = "Cancelled";
		

        // Pending Payment Status is where Order is placed and didn't complete the payment
        public const string PaymentStatusPending = "Pending";
        // Approved Payment Status is where Order is placed and complete the payment
        public const string PaymentStatusApproved = "Approved";
		// Delayed Payment Status is for Company Users where Payment can be made within 30 days 
		public const string PaymentStatusDelayedPayment = "Delayed Payment";
		// Payment Refunded when order is refunced
        public const string PaymentStatusRefunded = "Refunded";

        public const string SessionCart = "SessionShoppingCart";
	}
}
