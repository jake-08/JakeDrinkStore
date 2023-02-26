using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Models.ViewModels;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace JakeDrinkStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;

		[BindProperty] // Bind Property to use the ShoppingCartVM throughtout this controller
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

		#region Index Page
		public IActionResult Index()
        {
			// Get the login user id
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				ShoppingCartVM.OrderHeader.OrderTotal += (GetIndividualPrice(cart.Count, cart.Product.ListPrice) * cart.Count) + 
											(GetCasePrice(cart.CaseCount, cart.Product.MinBulkCase, cart.Product.CasePrize, cart.Product.BulkCasePrice) * cart.CaseCount);
			}

			return View(ShoppingCartVM);
        }

		public IActionResult IndividualPlus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartId);
			_unitOfWork.ShoppingCart.IncrementCount(cart, 1);
			_unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

		public IActionResult IndividualMinus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartId);
			if (cart.Count <= 1)
			{
				_unitOfWork.ShoppingCart.Remove(cart);
				if (cart.CaseCount == 0)
				{
                    ClearShoppingCartSession(cart);
                }
			}
			else
			{
				_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
			}
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

		public IActionResult IndividualRemove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartId);
			if (cart.CaseCount >= 1)
			{
				_unitOfWork.ShoppingCart.DecrementCount(cart, cart.Count);
            }
			else
			{
				_unitOfWork.ShoppingCart.Remove(cart);
                if (cart.CaseCount == 0)
                {
                    ClearShoppingCartSession(cart);
                }
            }
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

        public IActionResult CasePlus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCaseCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CaseMinus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartId);
            if (cart.CaseCount <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                if (cart.Count == 0)
                {
                    ClearShoppingCartSession(cart);
                }
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCaseCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

		public IActionResult CaseRemove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartId);
			if (cart.Count >= 1)
			{
				_unitOfWork.ShoppingCart.DecrementCaseCount(cart, cart.CaseCount);
			}
			else
			{
				_unitOfWork.ShoppingCart.Remove(cart);
                if (cart.Count == 0)
                {
                    ClearShoppingCartSession(cart);
                }
            }
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		private double GetIndividualPrice(int individualQuantity, double individualPrice)
		{
			if (individualQuantity > 0)
			{
				return individualPrice;
			}
			else
			{
				return 0;
			}
		}

		private double GetCasePrice(int caseQuantity, int minBulkCaseQuantity, double casePrice, double bulkPrice)
		{
			if (caseQuantity > minBulkCaseQuantity)
			{
				return bulkPrice;
			}
			else
			{
				return casePrice;
			}
		}

		private void ClearShoppingCartSession(ShoppingCart cart)
		{
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
        }
		#endregion Index Page

		#region Summary and Confirmation Page
		public IActionResult Summary()
		{
			// Get the login user id
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
				OrderHeader = new()
			};

			// Add personal details from Application User to Order Header
			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber ?? "";
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress ?? "";
			ShoppingCartVM.OrderHeader.Suburb = ShoppingCartVM.OrderHeader.ApplicationUser.Suburb ?? "";
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State ?? "";
			ShoppingCartVM.OrderHeader.Postcode = ShoppingCartVM.OrderHeader.ApplicationUser.Postcode ?? "";

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				ShoppingCartVM.OrderHeader.OrderTotal += (GetIndividualPrice(cart.Count, cart.Product.ListPrice) * cart.Count) +
											(GetCasePrice(cart.CaseCount, cart.Product.MinBulkCase, cart.Product.CasePrize, cart.Product.BulkCasePrice) * cart.CaseCount);
				cart.IndividualPrice = GetIndividualPrice(cart.Count, cart.Product.ListPrice);
				cart.CasePrice = GetCasePrice(cart.CaseCount, cart.Product.MinBulkCase, cart.Product.CasePrize, cart.Product.BulkCasePrice);
			}

			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPOST()
		{
			// Get the login user id
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			// Save the Shopping Cart to the Order Header Table 
			ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				ShoppingCartVM.OrderHeader.OrderTotal += (GetIndividualPrice(cart.Count, cart.Product.ListPrice) * cart.Count) +
											(GetCasePrice(cart.CaseCount, cart.Product.MinBulkCase, cart.Product.CasePrize, cart.Product.BulkCasePrice) * cart.CaseCount);
				cart.IndividualPrice = GetIndividualPrice(cart.Count, cart.Product.ListPrice);
				cart.CasePrice = GetCasePrice(cart.CaseCount, cart.Product.MinBulkCase, cart.Product.CasePrize, cart.Product.BulkCasePrice);
			}

			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			// Company User Logic
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
			// If it is not a Company User
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            }
			// If it is a Company User
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
				ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            // Add each shopping cart items to the Order Details Table
            foreach (var cart in ShoppingCartVM.ListCart)
			{
				OrderDetails orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					IndividualCount = cart.Count,
					IndividualPrice = cart.IndividualPrice,
					CaseCount = cart.CaseCount,
					CasePrice = cart.CasePrice,
				};
				_unitOfWork.OrderDetails.Add(orderDetail);
				_unitOfWork.Save();
			}

			// Once order headers and order details are saved to the database, clear the shopping cart
			_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
			_unitOfWork.Save();

			// Go to Stripe payment if it is not Company User, Compnay Users have 30 days delayed payment  
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				// Stripe Settings
				var domain = "https://localhost:44367/";
				var options = new SessionCreateOptions
				{
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
					CancelUrl = domain + $"customer/cart/index",
				};

				// Add items to SessionLineItemOptions List for Stripe 
				foreach (var item in ShoppingCartVM.ListCart)
				{
					if (item.Count > 0)
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
							Quantity = item.Count,
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

				// Create the Stripe Session
				var service = new SessionService();
				Session session = service.Create(options);

				// Save the SessionId to the OrderHeader
				ShoppingCartVM.OrderHeader.SessionId = session.Id;
				_unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, null);
				_unitOfWork.Save();

				// Go to Stripe Payment Page
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}
			// Go to Order Confirmation Page if it is Company User
			else
			{
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }
		}

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

			// Check if it not a company user to prevent updating the OrderHeader Payment Status to Approved 
			if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				// Check Stripe Payment Success or not 
				if (session.PaymentStatus.ToLower() == "paid")
				{
					// Update the PaymentIntentId
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
					// Update Order Status
					_unitOfWork.OrderHeader.UpdateStatus(id, SD.OrderStatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

            // Once Order is confirmed, send an email 
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Jake Book Store", "<p>New Order Created</p>");

            // Remove the items in the Shopping Carts 
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			return View(id);
		}
		#endregion Summary and Confirmation Page

	}
}
