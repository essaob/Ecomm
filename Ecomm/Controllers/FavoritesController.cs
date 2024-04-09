using System.Security.Claims;
using System.Threading.Tasks;
using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            // Retrieve the user ID from claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                // User is not authenticated, handle accordingly
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }

            // User is authenticated, retrieve the user ID
            string userId = claim.Value;

            // Check if the product is already in favorites for this user
            var favorite = _context.Favorites.Where(n=>n.ProductId == productId && n.ApplicationUserId==userId).FirstOrDefault();
            if (favorite != null)
            {
                // Item is already in favorites, handle accordingly (e.g., show message)
                TempData["success"] = "Already Added!";
                return RedirectToAction("Index", "Home"); // Redirect to home page or wherever appropriate
            }

            // If the item is not already in favorites, add it
            var newFavorite = new Favorite
            {
                ApplicationUserId = userId,
                ProductId = productId
            };
            TempData["success"] = "Added Successfully.";
            _context.Favorites.Add(newFavorite);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirect to home page or wherever appropriate
        }

    }
}
