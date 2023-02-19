using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace JakeDrinkStoreWeb.Areas.Customer.Controllers
{
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
            IEnumerable<ProductTag> productTagList = _unitOfWork.ProductTag.GetAll(pt => pt.ProductId == productId, includeProperties:"Tag");
            ShoppingCart cartObj = new()
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeProperties: "DrinkType,ProductTags"),
                ProductTag = productTagList,
                Count = 0,
                CaseCount = 0,
            };

            return View(cartObj);
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