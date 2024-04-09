



using Ecomm.Data;
using Ecomm.Models;
using Ecomm.Repository.IRepository;

namespace Ecomm.Repository
{
    public class SiteInfoRepository : Repository<SiteInfo>, ISiteInfoRepository
    {
        private ApplicationDbContext _db;
        public SiteInfoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(SiteInfo siteinfo)
        {
            _db.Update(siteinfo);
        }
    }
}
