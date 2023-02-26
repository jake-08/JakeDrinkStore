using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace JakeDrinkStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // The includeProperties have to be the same as Product Model Properties Name
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"DrinkType,ProductTags");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            IEnumerable<ProductTag> productTagList = _unitOfWork.ProductTag.GetAll(pt => pt.ProductId == productId, includeProperties: "Tag");
            // Can implement the logic where the product is already added to the shopping cart, show the count and caseCount properties 
            ShoppingCart cartObj = new()
            {
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeProperties: "DrinkType,ProductTags"),
                ProductTag = productTagList,
                Count = 1,
                CaseCount = 0,
            };

            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]  // only login users can add to the shopping Cart
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Get the login user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.ApplicationUserId == claim.Value && sc.ProductId == shoppingCart.ProductId);

            if (cartFromDb == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                // Add Http Integer Session and set key value pair for ShoppingCart count
                HttpContext.Session.SetInt32(
                    SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count
                );
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                _unitOfWork.ShoppingCart.IncrementCaseCount(cartFromDb, shoppingCart.CaseCount);
                _unitOfWork.Save();
            } 
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}