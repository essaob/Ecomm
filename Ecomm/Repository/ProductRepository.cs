
using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Repository.IRepository;

namespace Ecomm.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public bool ProductExistsWithSlug(string slug, int excludeProductId = 0)
        {
            return _db.Products.Any(p => p.Id != excludeProductId && p.Slug == slug);
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(u=>u.Id == product.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = product.Title;
                objFromDb.Description = product.Description;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.Price = product.Price;
                objFromDb.Author = product.Author;
                if(objFromDb.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }

                
            }
        }
    }
}
