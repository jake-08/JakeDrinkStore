using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using JakeDrinkStore.Models.ViewModels;
using JakeDrinkStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace JakeDrinkStoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // IWebHostEnvironment to access wwwroot folder 
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        // GET
        /// <summary>
        /// Create and Update Product
        /// </summary>
        public IActionResult Upsert(int? id)
        {
            // Create Product
            if (id == null || id == 0)
            {
                ProductVM productVM = new()
                {
                    Product = new(),
                    TagList = _unitOfWork.Tag.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString(),
                    }),
                    DrinkTypeList = _unitOfWork.DrinkType.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString(),
                    })
                };
                return View(productVM);
            }
            // Update Product
            else
            {
                // Get the product including ProductTags based on id 
                Product? product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "ProductTags");

                if (product == null) 
                {
                    return NotFound();
                }

                ProductVM productVM = new()
                {
                    Product = product,
                    // Get the existing Tags to populate, Drink Type will be populated from Product model 
                    TagIds = product.ProductTags.Select(pt => pt.TagId).ToList(),
                    // Get all the Tags for dropdown options
                    TagList = _unitOfWork.Tag.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString(),
                    }),
                    // Get all the Drink Types for dropdown options
                    DrinkTypeList = _unitOfWork.DrinkType.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString(),
                    })
                };
                return View(productVM);
            }
        }

        // POST 
        /// <summary>
        /// Create and Update Product
        /// Can impletment Default Image functionality, by giving choose Default Image Button in UI 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Upload new image to the folder and assign  ImageUrl to the Product object
                // E.g. C:\\Users\\JakeLin\\Documents\\ASP.NET\\JakeDrinkStore\\JakeDrinkStoreWeb\\wwwroot
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\drink-images");
                    var extension = Path.GetExtension(file.FileName);

                    // Check existing images, if yes, replace it
                    if (obj.Product.ImageUrl != null)
                    {
                        // Trim to remove the Product.ImageUrl first backslash for Combine to work
                        // E.g. \\images\\drink-images\\6eaa56c5-3783-4350-a18b-a9a9aa9279ee.jpeg -> images\\drink-images\\6eaa56c5-3783-4350-a18b-a9a9aa9279ee.jpeg
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Create FileStream object to copy the file over
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\drink-images\" + fileName + extension;
                }    

                // Create Product
                if (obj.Product.Id == 0)
                {
                    foreach (var tag in obj.TagIds)
                    {
                        var productTag = new ProductTag()
                        {
                            Product = obj.Product,
                            // We can use Tag object or TagId to assign Tag, for this TagId is used to avoid ununecessary database call. 
                            // Tag = _unitOfWork.Tag.GetFirstOrDefault(t => t.Id == tag),
                            TagId = tag
                        };
                        obj.Product.ProductTags.Add(productTag);
                    }
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully";
                }
                // Update Product
                else
                {
                    // Get the product including ProductTags based on id 
                    Product product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == obj.Product.Id, includeProperties: "ProductTags");

                    foreach (var tag in obj.TagIds)
                    {
                        var productTag = new ProductTag()
                        {
                            Product = obj.Product,
                            // We can use Tag object or TagId to assign Tag, for this TagId is used to avoid ununecessary database call. 
                            // Tag = _unitOfWork.Tag.GetFirstOrDefault(t => t.Id == tag),
                            TagId = tag
                        };
                        obj.Product.ProductTags.Add(productTag);
                    }
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        #region API CALLS
        // GET
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "DrinkType,ProductTags").OrderByDescending(product => product.Id);
            return Json(new { data = productList });
        }

        // POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Cannot be deleted - the product doesn't exist" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            // This will remove the product in the ProductTag Table as well 
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Successfully Deleted" });

            //return RedirectToAction("Index");
        }
        #endregion API CALLS
    }
}
