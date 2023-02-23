using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JakeDrinkStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
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
			};

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				ShoppingCartVM.CartTotal += (GetIndividualPrice(cart.Count, cart.Product.ListPrice) * cart.Count) + 
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

		#region Summary Page
		public IActionResult Summary()
		{
			// Get the login user id
			//	var claimsIdentity = (ClaimsIdentity)User.Identity;
			//	var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			//	ShoppingCartVM = new ShoppingCartVM()
			//	{
			//		ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
			//	};

			//	foreach (var cart in ShoppingCartVM.ListCart)
			//	{
			//		ShoppingCartVM.CartTotal += (GetIndividualPrice(cart.Count, cart.Product.ListPrice) * cart.Count) +
			//									(GetCasePrice(cart.CaseCount, cart.Product.MinBulkCase, cart.Product.CasePrize, cart.Product.BulkCasePrice) * cart.CaseCount);
			//	}

			//	return View(ShoppingCartVM);
			return View();
		}
		#endregion Summary Page

	}
}
