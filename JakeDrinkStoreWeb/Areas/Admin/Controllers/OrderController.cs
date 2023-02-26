using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Models.ViewModels;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace JakeDrinkStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // Bind OrderVM to use thorughout the controller 
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderHeaderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(u => u.OrderHeaderId == orderHeaderId, includeProperties: "Product")
            };
            return View(OrderVM);
        }

		[HttpPost]
		[ActionName("Details")]
		[ValidateAntiForgeryToken]
		public IActionResult Details_Pay_Now()
		{
			OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
			OrderVM.OrderDetails = _unitOfWork.OrderDetails.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

			// Stripe Settings
			var domain = "https://localhost:44367/";
			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderHeaderId={OrderVM.OrderHeader.Id}",
			};

			// Add items to SessionLineItemOptions List for Stripe 
			foreach (var item in OrderVM.OrderDetails)
			{
				if (item.IndividualCount > 0)
				{
					var sessionLineItemIndividual = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.IndividualPrice * 100), // 20.00 -> 2000
							Currency = "aud",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Name
							},
						},
						Quantity = item.IndividualCount,
					};
					options.LineItems.Add(sessionLineItemIndividual);
				}
				if (item.CaseCount > 0)
				{
					var sessionLineItemCase = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.CasePrice * 100), // 20.00 -> 2000
							Currency = "aud",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Name
							},
						},
						Quantity = item.CaseCount,
					};
					options.LineItems.Add(sessionLineItemCase);
				}
			}

			var service = new SessionService();
			Session session = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

        public IActionResult PaymentConfirmation(int orderHeaderId)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderId);
			// For Company Users
			if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				// Check Stripe Status 
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, orderHeader.SessionId, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}
			// For users who didn't complete the payment
			if (orderHeader.PaymentStatus == SD.PaymentStatusPending)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				// Check Stripe Status 
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, orderHeader.SessionId, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, SD.OrderStatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

			return View(orderHeaderId);
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails(int orderHeaderId)
		{
            // Don't allow EF to track the entity when retrieving it
			var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
			orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.Suburb = OrderVM.OrderHeader.Suburb;
			orderHeaderFromDb.State = OrderVM.OrderHeader.State;
			orderHeaderFromDb.Postcode = OrderVM.OrderHeader.Postcode;

			if (OrderVM.OrderHeader.Carrier != null)
			{
				orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}
			if (OrderVM.OrderHeader.TrackingNumber != null)
			{
				orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

            // Have to use Update function to update the entity 
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            // If tracked it true, we can just call the Save function without Update function
            _unitOfWork.Save();
			TempData["Success"] = "Order Details Updated Successfully.";
			return RedirectToAction("Details", "Order", new { orderHeaderId = orderHeaderFromDb.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.OrderStatusInProcess);
			_unitOfWork.Save();
			TempData["Success"] = "Order Started Processing Successfully.";
			return RedirectToAction("Details", "Order", new { orderHeaderId = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.OrderStatusCompleted;
			orderHeader.ShippingDate = DateTime.Now;
			if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}

			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Order Shipped Successfully.";
			return RedirectToAction("Details", "Order", new { orderHeaderId = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
			if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};
				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderStatusCancelled, SD.PaymentStatusRefunded);
			}
			else
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.OrderStatusCancelled, SD.OrderStatusCancelled);
			}
			_unitOfWork.Save();

			TempData["Success"] = "Order Cancelled Successfully.";
			return RedirectToAction("Details", "Order", new { orderHeaderId = OrderVM.OrderHeader.Id });
		}

		#region API Calls
		/// <summary>
		/// Get All will only show Order Headers 
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            // Only Admin can see all the orders 
            if (User.IsInRole(SD.Role_Admin))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending || u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.OrderStatusInProcess);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.OrderStatusApproved);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.OrderStatusCompleted);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        [HttpDelete]
        public IActionResult Delete(int orderHeaderId)
        {
            // Get the OrderHeader
            OrderHeader? orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(orderHeader => orderHeader.Id == orderHeaderId);

            if (orderHeaderFromDb == null)
            {
                return Json(new { success = false, message = "Cannot be deleted - Order doesn't exist" });
            }

            // Get all the Order Details based on the Order Header and Remove Range
            IEnumerable<OrderDetails> orderDetailsListFromDb = _unitOfWork.OrderDetails.GetAll(orderDetails => orderDetails.OrderHeaderId == orderHeaderId);
            _unitOfWork.OrderDetails.RemoveRange(orderDetailsListFromDb);
            // Remove OrderHeader from the Db
            _unitOfWork.OrderHeader.Remove(orderHeaderFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Order Successfully Deleted" });
        }
        #endregion API Calls
    }
}
