using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ecomm.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            // migration if not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Migration error: {ex.Message}");
            }

            // Seed default data
            SeedRoles();
            SeedAdminUser();
            SeedSiteInfo();
        }

        private void SeedRoles()
        {
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            }
        }

        private void SeedAdminUser()
        {
            var adminEmail = "Admin1@gmail.com";

            if (_userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult() == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 Bialek",
                    State = "IL",
                    PostalCode = "8413301",
                    City = "BeerSheva",
                };

                var result = _userManager.CreateAsync(adminUser, "Admin1@gmail.com").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    var user = _userManager.FindByEmailAsync(adminUser.Email).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
                else
                {
                    Console.WriteLine($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        private void SeedSiteInfo()
        {
            if (!_db.SiteInfo.Any())
            {
                var siteInfo = new SiteInfo
                {
                    WebsiteName = "Home made harmony online",
					EmailAddress = "HouseholdProduct@gamil.com",
                    PhoneNumber = "0512345678",
                    Address = "Alnor Street, Rahat, Israil"
                };

                _db.SiteInfo.Add(siteInfo);
                _db.SaveChanges();
            }
        }
    }
}
