using JakeDrinkStore.DataAccess;
using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JakeDrinkStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class TagController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TagController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        // GET
        public IActionResult Index()
        {
            IEnumerable<Tag> objCategoryList = _unitOfWork.Tag.GetAll().OrderByDescending(tag => tag.CreatedDateTime); ;
            return View(objCategoryList);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag obj)
        {
            bool isExist = await _unitOfWork.Tag.AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "The Tag Name already exists.");
                //return BadRequest("The Tag Name already exists");
            }

            if (ModelState.IsValid && !isExist)
            {
                _unitOfWork.Tag.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Tag created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Tag? tagFromDb = _unitOfWork.Tag.GetFirstOrDefault(c => c.Id == id);

            if (tagFromDb == null)
            {
                return NotFound();
            }

            return View(tagFromDb);
        }

        // POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tag obj)
        {
            bool isSameName = await _unitOfWork.Tag.AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());
            bool isSameDate = await _unitOfWork.Tag.AnyAsync(c => c.CreatedDateTime == obj.CreatedDateTime);

            if (isSameName && isSameDate)
            {
                ModelState.AddModelError("Name", "The Tag Name already exists.");
                ModelState.AddModelError("CreatedDateTime", "The Tag Created Date is the same.");
            }

            if (ModelState.IsValid && (!isSameName || !isSameDate))
            {
                _unitOfWork.Tag.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Tag updated successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET 
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Tag? tagFromDb = _unitOfWork.Tag.GetFirstOrDefault(c => c.Id == id);

            if (tagFromDb == null)
            {
                return NotFound();
            }

            return View(tagFromDb);
        }

        // POST 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var categoryFromDb = _unitOfWork.Tag.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.Tag.Remove(categoryFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Tag deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
