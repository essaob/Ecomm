using Ecomm.Models;
using Ecomm.Repository;
using Ecomm.Repository.IRepository;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IUniteOfWork _unitofwork;
        public DashboardController(IUniteOfWork uniteOfWork)
        {
            _unitofwork = uniteOfWork;
        }
        public IActionResult Index()
        {
            var order_total = _unitofwork.OrderHeader.GetAll().Count();
            var product_total = _unitofwork.Product.GetAll().Count();
            var category_total = _unitofwork.Category.GetAll().Count();
            ViewBag.OrderTotal = order_total;
            ViewBag.ProductTotal = product_total;
            ViewBag.CategoryTotal = category_total;

            return View();
        }
        public IActionResult Setting()
        {
            var data = _unitofwork.Siteinfo.GetFirstOrDefault(u => u.Id == 1);
            return View(data);
        }

        [HttpPost]
        public IActionResult Setting(SiteInfo obj)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.Siteinfo.Update(obj);
                _unitofwork.Save(); 

                TempData["success"] = "Site Info Updated Successfully.";
                return View(); 
            }

            // If ModelState is not valid, return to the same view with validation errors
            return View(obj);
        }
    }
}
