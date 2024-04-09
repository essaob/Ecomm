



using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Repository.IRepository;

namespace Ecomm.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

      
    }
}
