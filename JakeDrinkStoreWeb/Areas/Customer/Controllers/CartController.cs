﻿using JakeDrinkStore.DataAccess.Repository.IRepository;
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
		[BindProperty] // Bind Property to use the ShoppingCartVM throughtout this controller
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

			ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
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

			// Add Order Details for each item
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

			// Stripe Settings
			var domain = "https://localhost:44367/";
			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
				CancelUrl = domain + $"customer/cart/index",
			};

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

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

			if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				// Check Stripe Payment Success or not 
				if (session.PaymentStatus.ToLower() == "paid")
				{
					// Update the PaymentIntentid and Order Status
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			return View(id);
		}
		#endregion Summary and Confirmation Page

	}
}
