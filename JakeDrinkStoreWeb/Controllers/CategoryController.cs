using JakeDrinkStore.DataAccess;
using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace JakeDrinkStoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _db;
        public CategoryController(ICategoryRepository db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _db.GetAll();
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
        public async Task<IActionResult> Create(Category obj)
        {
            bool isExist = await _db.AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "The Category Name already exists.");
                //return BadRequest("The Category Name already exists");
            }
            
            if (ModelState.IsValid && !isExist)
            {
                _db.Add(obj);
                _db.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET
        public IActionResult Edit(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            Category? categoryFromDb = _db.GetFirstOrDefault(c => c.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        // POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category obj)
        {
            bool isSameName = await _db.AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());
            bool isSameDate = await _db.AnyAsync(c => c.CreatedDateTime == obj.CreatedDateTime);

            if (isSameName && isSameDate)
            {
                ModelState.AddModelError("Name", "The Category Name already exists.");
                ModelState.AddModelError("CreatedDateTime", "The Category Date is the same.");
            }

            if (ModelState.IsValid && (!isSameName || !isSameDate))
            {
                _db.Update(obj);
                _db.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // GET 
        public IActionResult Delete(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            Category? categoryFromDb = _db.GetFirstOrDefault(c => c.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        // POST 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(Guid? id)
        {
            var categoryFromDb = _db.GetFirstOrDefault(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            _db.Remove(categoryFromDb); 
            _db.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
