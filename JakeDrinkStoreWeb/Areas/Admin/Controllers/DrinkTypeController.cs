using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace JakeDrinkStoreWeb.Areas.Admin.Controllers
{
    public class DrinkTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DrinkTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET
        public IActionResult Index()
        {
            IEnumerable<DrinkType> drinkTypeList = _unitOfWork.DrinkType.GetAll();
            return View(drinkTypeList);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DrinkType obj)
        {
            bool isExist = await _unitOfWork.DrinkType.AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "The Drink Type already exists.");
            }

            if (ModelState.IsValid && !isExist)
            {
                _unitOfWork.DrinkType.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Drink Type created successfully";
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

            DrinkType? drinkTypeFromDb = _unitOfWork.DrinkType.GetFirstOrDefault(c => c.Id == id);

            if (drinkTypeFromDb == null)
            {
                return NotFound();
            }

            return View(drinkTypeFromDb);
        }

        // POST 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DrinkType obj)
        {
            bool isSameName = await _unitOfWork.DrinkType.AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());
            bool isSameDate = await _unitOfWork.DrinkType.AnyAsync(c => c.CreatedDateTime == obj.CreatedDateTime);

            if (isSameName && isSameDate)
            {
                ModelState.AddModelError("Name", "The Drink Type already exists.");
                ModelState.AddModelError("CreatedDateTime", "The Drink Type Created Date is the same.");
            }

            if (ModelState.IsValid && (!isSameName || !isSameDate))
            {
                _unitOfWork.DrinkType.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Drink Type updated successfully";
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

            DrinkType? drinkTypeFromDb = _unitOfWork.DrinkType.GetFirstOrDefault(c => c.Id == id);

            if (drinkTypeFromDb == null)
            {
                return NotFound();
            }

            return View(drinkTypeFromDb);
        }

        // POST 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(Guid? id)
        {
            var drinkTypeFromDb = _unitOfWork.DrinkType.GetFirstOrDefault(u => u.Id == id);

            if (drinkTypeFromDb == null)
            {
                return NotFound();
            }

            _unitOfWork.DrinkType.Remove(drinkTypeFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Drink Type deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
