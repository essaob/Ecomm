using System.Diagnostics;
using System.Security.Claims;
using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Repository;
using Ecomm.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Ecomm.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUniteOfWork _uniteOfWork;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, IUniteOfWork uniteOfWork, ApplicationDbContext context)
        {
            _logger = logger;
            _uniteOfWork = uniteOfWork;
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productsList = _uniteOfWork.Product.GetAll(includeProperties: "Category");

            return View(productsList);
        }

        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = _context.StockOutNotifies
                .Where(n => n.ApplicationUserId == userId)
                .ToList();
            return View(notifications);
        }

        // Action for filtering products
        public IActionResult Filter(string category, string sortBy, string color, string priceRange)
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Category);

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.Name == category);
            }

            // Filter by color
            if (!string.IsNullOrEmpty(color))
            {
                products = products.Where(p => p.Color == color);
            }

            // Filter by price range
            if (!string.IsNullOrEmpty(priceRange))
            {
                var rangeParts = priceRange.Split('-');
                if (rangeParts.Length == 2 && double.TryParse(rangeParts[0], out double minPrice) && double.TryParse(rangeParts[1], out double maxPrice))
                {
                    products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
            }

            var categories = _context.Category.Select(c => c.Name).Distinct().ToList();
            ViewBag.Categories = categories;

            // Sort by price
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "price_increase":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "price_decrease":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                    case "most_popular":
                        products = products.OrderByDescending(p => p.Views);
                        break;
                    default:
                        break;
                }
            }

            return View("Shop", products.ToList());
        }

        public IActionResult Shop()
        {
            IEnumerable<Product> productsList = _uniteOfWork.Product.GetAll(includeProperties: "Category");
            var categories = _context.Category.Select(c => c.Name).Distinct().ToList();
            ViewBag.Categories = categories;
            return View(productsList);
        }

        public IActionResult Details(int productId)
        {
            // Retrieve the product with the specified productId along with its category
            var product = _uniteOfWork.Product.GetFirstOrDefault(
                u => u.Id == productId,
                includeProperties: "Category"
            );
            product.Views++;
            _uniteOfWork.Save();

            var relatedProducts = _context.Products
                .Where(p => p.CategoryId == product.CategoryId)
                .Take(4).ToList();

            var total_purchase = _context.OrderDetails.Count(u => u.ProductId == product.Id);

            // Store both the main product and related products in ViewData
            ViewData["MainProduct"] = product;
            ViewData["RelatedProducts"] = relatedProducts;
            ViewData["total_purchase"] = total_purchase;

            return View();
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, string button = null, int quantity = 1)
        {
            var _check_quantity = _context.Products.Where(n => n.Id == productId).FirstOrDefault();

            if (_check_quantity != null)
            {
                if (_check_quantity.Quantity < quantity)
                {
                    TempData["error"] = "Product Stock Out.";
                    // Add logging here to see if this branch is being executed
                    Console.WriteLine("Product stock is insufficient.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Cart handling logic
                    var cart = HttpContext.Session.GetString("cart");

                    if (cart == null)
                    {
                        // Create new cart
                        var product = getDetailProduct(productId);
                        List<Cart> listCart = new List<Cart>()
                {
                    new Cart
                    {
                        Product = product,
                        Quantity = quantity
                    }
                };
                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
                    }
                    else
                    {
                        // Update existing cart
                        List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                        bool check = true;

                        for (int i = 0; i < dataCart.Count; i++)
                        {
                            if (dataCart[i].Product.Id == productId)
                            {
                                dataCart[i].Quantity += quantity;
                                check = false;
                            }
                        }

                        if (check)
                        {
                            dataCart.Add(new Cart
                            {
                                Product = getDetailProduct(productId),
                                Quantity = 1
                            });
                        }

                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                    }

                    TempData["success"] = "Item Added to Cart.";

                    // Redirect based on button parameter
                    if (button == "buy_now")
                    {
                        return RedirectToAction("Summary", "Cart");
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                // Product not found
                TempData["error"] = "Product not found.";
                // Add logging here to see if _check_quantity is null
                Console.WriteLine("Product not found.");
                return RedirectToAction(nameof(Index));
            }
        }




        private Product getDetailProduct(int id)
        {
            var product = _context.Products.Find(id);
            return product;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart shoppingCart, string button)
        {
            if (User.Identity.IsAuthenticated){
                var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
          
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = _uniteOfWork.ShoppingCart.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId
                );
                if (cartFromDb == null)
                {
                    _uniteOfWork.ShoppingCart.Add(shoppingCart);
                }
                else
                {
                    _uniteOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                }
            }
                Product product = _uniteOfWork.Product.GetFirstOrDefault(u => u.Id == shoppingCart.ProductId); // Fetch the product from the database
     
            // Decrease the product quantity
            if (product != null)
            {
                product.Quantity -= shoppingCart.Count;
                _uniteOfWork.Product.Update(product);
            }

            _uniteOfWork.Save();

        if(button != null && button== "add_to_cart")
            {
                TempData["success"] = "Added To Cart Successfully.";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Summary", "Cart");
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult NotifyMe(StockOutNotify stockOutNotify)
        {
            
                // Add logic to handle the notification request
                _context.Add(stockOutNotify);

                TempData["success"] = "Notification request received. We will notify you when the product is back in stock.";
                return RedirectToAction(nameof(Index));
          
        }
        [Authorize]
        public IActionResult Favorite()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var favorites = _context.Favorites
                    .Where(n => n.ApplicationUserId == claim.Value).Include("Product")
                    .ToList();

                return View(favorites);
            }

            return NotFound();
        }
        public IActionResult AboutUs()
        {
            return View();
        }    
        public IActionResult ContactUs()
        {
            var Contact = _uniteOfWork.Siteinfo.GetFirstOrDefault(u => u.Id == 1);
            return View(Contact);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
