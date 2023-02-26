using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JakeDrinkStoreWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get Login User
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // If the user is logged in, get the Session
            if (claim != null)
            {
                // Check if the session has value
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                // If session doesn't have a value, set session shopping cart count value
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            // Clear the Session when the user logged out
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
