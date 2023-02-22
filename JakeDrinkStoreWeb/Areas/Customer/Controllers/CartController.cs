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
	}
}
