using Ecomm.Models;
using System.Text.RegularExpressions;
using Ecomm.Models.ViewModels;
using Ecomm.Repository;
using Ecomm.Repository.IRepository;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ecomm.Data;


namespace Ecomm.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUniteOfWork _unitofwork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;

        public ProductController(IUniteOfWork uniteofwork, IWebHostEnvironment hostEnvironment, ApplicationDbContext context)
        {
            _unitofwork = uniteofwork;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                product = new(),
                CategoryList = _unitofwork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })

            };

		

			if (id == null || id == 0 )
            {
				//create product
					return View(productVM);
            }
            else
            {
                //Update Product
                productVM.product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id);
            }
            return View(productVM);
        }
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);
                    if (obj.product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                string generatedSlug = GenerateUniqueSlug(obj.product);
                obj.product.Slug = generatedSlug;
                if (obj.product.Id == 0 )
                {
                  //  if (ModelState.IsValid)
                    
                        _unitofwork.Product.Add(obj.product);
                        _unitofwork.Save();
                    
				}
                
                else
                {
                    _context.Products.Update(obj.product);
                    _context.SaveChanges();
                }


         
                TempData["success"] = "Product Updated Successfully.";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        private string GenerateUniqueSlug(Product product)
        {
            string titleSlug = Regex.Replace(product.Title, @"[^a-zA-Z0-9\s-]", "").Trim().ToLower();
            titleSlug = Regex.Replace(titleSlug, @"\s+", "-");

            string uniqueSlug = titleSlug;
            int i = 1;
            while (_unitofwork.Product.ProductExistsWithSlug(uniqueSlug, product.Id))
            {
                uniqueSlug = $"{titleSlug}-{i}";
                i++;
            }
            return uniqueSlug;
        }



        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitofwork.Product.GetAll(includeProperties: "Category");
            return Json(new { data = productList });
        }

        //post
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitofwork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            if (obj.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            _unitofwork.Product.Remove(obj);
            _unitofwork.Save();

            return Json(new { success = true, message = "Product Deleted Successfully." });
        }
        #endregion  
    }
}
