using Ecomm.Clients;
using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Models.ViewModels;
using Ecomm.Repository.IRepository;
using Ecomm.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PayPal.Api;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecomm.Controllers
{

    public class CartController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly IUniteOfWork  _uniteofwork;
        private readonly ApplicationDbContext  _context;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUniteOfWork uniteOfWork, ApplicationDbContext context, PaypalClient paypalClient)
        {
            _uniteofwork = uniteOfWork;
            _context = context;
            _paypalClient = paypalClient;
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetString("cart");
       
            if (!string.IsNullOrEmpty(cart))
            {
                try
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                    if (dataCart.Count > 0)
                    {
                        
                      
                        return View(dataCart);
                    }
                }
                catch (JsonException ex)
                {
              
                }
            }

            return View();
        }


        private List<Product> GetProductsByIds(List<int> productIds)
        {
            var products = new List<Product>();
            var processedIds = new HashSet<int>(); // Track processed IDs

            try
            {
                foreach (var id in productIds)
                {
                    // Check if the ID has already been processed
                    if (processedIds.Contains(id))
                    {
                        continue; // Skip this ID if it's already processed
                    }

                    var _product = _context.Products.FirstOrDefault(n => n.Id == id);
                    if (_product != null)
                    {
                        products.Add(_product);
                        processedIds.Add(id); // Add the processed ID to the HashSet
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                // Example: Console.WriteLine($"Error fetching products by IDs: {ex.Message}");
            }

            return products;
        }






        public IActionResult Plus(int cartId)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (!string.IsNullOrEmpty(cart))
            {
                try
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                    Cart item = dataCart.FirstOrDefault(x => x.Product.Id == cartId);
                    if (item != null)
                    {
                        // Increment the count for the item
                        item.Quantity++;
                        // Update the session data with the modified cart
                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                    }
                }
                catch (JsonException ex)
                {
                    // Log or handle the exception
                    // For example:
                    // Console.WriteLine($"Error deserializing cart data: {ex.Message}");
                }
            }
            TempData["success"] = "Cart Updated Successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (!string.IsNullOrEmpty(cart))
            {
                try
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                    Cart item = dataCart.FirstOrDefault(x => x.Product.Id == cartId);
                    if (item != null)
                    {
                        // Decrement the count for the item
                        item.Quantity--;
                        if (item.Quantity <= 0)
                        {
                            // Remove the item from the cart if the quantity becomes zero or negative
                            dataCart.Remove(item);
                        }
                        // Update the session data with the modified cart
                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                    }
                }
                catch (JsonException ex)
                {
                    // Log or handle the exception
                    // For example:
                    // Console.WriteLine($"Error deserializing cart data: {ex.Message}");
                }
            }
            TempData["success"] = "Cart Updated Successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (!string.IsNullOrEmpty(cart))
            {
                try
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                    Cart itemToRemove = dataCart.FirstOrDefault(x => x.Product.Id == cartId);
                    if (itemToRemove != null)
                    {
                        // Remove the item from the cart
                        dataCart.Remove(itemToRemove);
                        // Update the session data with the modified cart
                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                    }
                }
                catch (JsonException ex)
                {
                  
                }
            }
            TempData["success"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            ViewBag.ClientId = _paypalClient.ClientId;
            if (User.Identity.IsAuthenticated)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    OrderHeader = new()
                };
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartVM.OrderHeader.ApplicationUser = _uniteofwork.ApplicationUser.GetFirstOrDefault(
                u => u.Id == claim.Value);

                ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
                ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.Name;
                ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.City;
                ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.State;
                ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber;
                ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.StreetAddress;
                ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.PostalCode;
            }
            var cart = HttpContext.Session.GetString("cart");

            if (!string.IsNullOrEmpty(cart))
            {
                try
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                    if (dataCart.Count > 0)
                    {
                        var total_quantity = 0;
                        decimal total_price = 0;
                        foreach (var item in dataCart)
                        {
                            total_quantity += item.Quantity;
                            total_price = total_price + (decimal)item.Product.Price * item.Quantity;
                        }

                        // Instantiate ShoppingCartVM
                        ShoppingCartVM ShoppingCartVM = new ShoppingCartVM();
                        ShoppingCartVM.Quantity = total_quantity;
                        ShoppingCartVM.Total_Price = total_price;

                        return View(ShoppingCartVM);
                    }
                }
                catch (JsonException ex)
                {
                   
                }
            }

            TempData["success"] = "Cart is Empty.";
            return RedirectToAction("Index", "Home");
        }

   
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost(ShoppingCartVM shoppingCartVM)
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            }

            if (ModelState.IsValid)
            {
                // Generate or obtain the encryption key and IV
                byte[] encryptionKey = GenerateEncryptionKey(); // Generate a key of appropriate size
                byte[] iv = GenerateInitializationVector(); // Generate a suitable IV

                byte[] encryptedBytes = AesEncryption.Encrypt_AES(shoppingCartVM.OrderHeader.CardNumber, encryptionKey, iv);
                string encryptedCartNumber = Convert.ToBase64String(encryptedBytes);
               

                // Encrypt CardNumber and CardCSV
                ShoppingCartVM.OrderHeader.CardNumber = encryptedCartNumber;
                ShoppingCartVM.OrderHeader.CardCSV = shoppingCartVM.OrderHeader.CardCSV;

                ShoppingCartVM.OrderHeader.CardExpiryDate = shoppingCartVM.OrderHeader.CardExpiryDate;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.Name;
                ShoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.City;
                ShoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.State;
                ShoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.PhoneNumber;
                ShoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.StreetAddress;
                ShoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.PostalCode;

                var cart = HttpContext.Session.GetString("cart");
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                double total_price = 0;
                int total_quantity = 0;
                foreach (var item in dataCart)
                {
                    total_quantity += item.Quantity;
                    total_price = total_price + (double)item.Product.Price * item.Quantity;
                }
                ShoppingCartVM.OrderHeader.OrderTotal = total_price;
                if (User.Identity.IsAuthenticated)
                {
                    ShoppingCartVM.OrderHeader.TrackingId =  GenerateRandomString(10); // Change 10 to the desired length
                }

                _uniteofwork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
                _uniteofwork.Save();

                foreach (var item_n in dataCart)
                {
                    OrderDetail orderDetail = new()
                    {
                        ProductId = item_n.Product.Id,
                        OrderId = ShoppingCartVM.OrderHeader.Id,
                        Price = item_n.Product.Price,
                        Count = total_quantity
                    };

                    _uniteofwork.OrderDetails.Add(orderDetail);
                    _uniteofwork.Save();
                }

                // Update product quantities
                foreach (var item_n in dataCart)
                {
                    // Retrieve the product from the database
                    var product = _context.Products.Find(item_n.Product.Id);
                    if (product != null)
                    {
                        // Deduct the purchased quantity from the available quantity
                        product.Quantity -= item_n.Quantity;

                        // Save the updated product back to the database
                        _context.Products.Update(product);
                        _context.SaveChanges(); // Save changes immediately after each update
                    }
                    
                }

                HttpContext.Session.Remove("cart");
                _uniteofwork.Save();

                return RedirectToAction(nameof(ThankYou));
            }
            return View(shoppingCartVM);
        }

        private string GenerateRandomString(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(validChars[rnd.Next(validChars.Length)]);
            }

            return sb.ToString();
        }



        public IActionResult ThankYou()
        {
            return View();  
        }

        // Method to generate a random encryption key (example only, replace with your own key generation method)
        static byte[] GenerateEncryptionKey()
        {
            // Generate a random byte array of appropriate size (e.g., 256 bits for AES-256)
            byte[] key = new byte[32]; // 32 bytes = 256 bits
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        // Method to generate a random initialization vector (IV) (example only, replace with your own IV generation method)
        static byte[] GenerateInitializationVector()
        {
            // Generate a random byte array of appropriate size (e.g., 128 bits for AES)
            byte[] iv = new byte[16]; // 16 bytes = 128 bits
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }
      
    }
}
