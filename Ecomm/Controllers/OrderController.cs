using Ecomm.Models;
using Ecomm.Models.ViewModels;
using Ecomm.Repository;
using Ecomm.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Controllers
{

    public class OrderController : Controller
    {
        private readonly IUniteOfWork _uniteOfWork;
        public OrderVM OrderVM { get; set; }
        public OrderController(IUniteOfWork uniteOfWork)
        {
            _uniteOfWork = uniteOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders;

            orderHeaders = _uniteOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
