using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Models.ViewModels;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace JakeDrinkStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ApplicationUserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationUserController(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        // GET
        public IActionResult Create()
        {
            string registerUrl = "/Identity/Account/Register";
            return Redirect(registerUrl);
        }

        // GET
        /// <summary>
        /// Edit Application User
        /// </summary>
        public IActionResult Edit(string? id)
        {
            ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);

            ApplicationUserVM userVM = new()
            {
                Id = user.Id,
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                Suburb = user.Suburb,
                State = user.State,
                Postcode = user.Postcode,
                PhoneNumber = user.PhoneNumber,
            };

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                return View(user);
            }
        }

        // POST 
        /// <summary>
        /// Edit Application User
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    ApplicationUser dbUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == user.Id);

                    dbUser.Name = user.Name;
                    dbUser.StreetAddress = user.StreetAddress;
                    dbUser.Suburb = user.Suburb;
                    dbUser.State = user.State;
                    dbUser.Postcode = user.Postcode;
                    dbUser.PhoneNumber = user.PhoneNumber;

                    _unitOfWork.ApplicationUser.Update(dbUser);
                    TempData["success"] = "User updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        #region API CALLS
        // GET
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _unitOfWork.ApplicationUser.GetAll().OrderByDescending(user => user.Id);
            return Json(new { data = userList });
        }

        // POST
        [HttpDelete]
        public IActionResult Delete(Guid? id)
        {
            ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == id.ToString());
            if (user == null)
            {
                return Json(new { success = false, message = "Cannot be deleted - User doesn't exist" });
            } 

            _unitOfWork.ApplicationUser.Remove(user);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Successfully Deleted" });
        }
        #endregion API CALLS
    }
}
