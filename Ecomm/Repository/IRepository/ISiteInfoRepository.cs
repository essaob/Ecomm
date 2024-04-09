

using Ecomm.Models;

namespace Ecomm.Repository.IRepository
{
    public interface ISiteInfoRepository : IRepository<SiteInfo>
    {
        void Update(SiteInfo siteinfo);
    }
}
