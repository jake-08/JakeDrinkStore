using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JakeDrinkStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        // GET
        /// <summary>
        /// Create and Update Company
        /// </summary>
        public IActionResult Upsert(int? id)
        {
            Company company = new(); 

            // Create Company
            if (id == null || id == 0)
            {
                return View(company);
            }
            // Update Company
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
                return View(company);
            }
        }

        // POST 
        /// <summary>
        /// Create and Update Company
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                // Create Company
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company added successfully";
                }
                // Update Company
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        #region API CALLS
        // GET
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll().OrderByDescending(company => company.Id);
            return Json(new { data = companyList });
        }

        // POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
            if (company == null)
            {
                return Json(new { success = false, message = "Cannot be deleted - Company doesn't exist" });
            } 

            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Successfully Deleted" });
        }
        #endregion API CALLS
    }
}
