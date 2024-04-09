



using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Repository.IRepository;

namespace Ecomm.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            _db.Category.Update(category);
        }
    }
}
