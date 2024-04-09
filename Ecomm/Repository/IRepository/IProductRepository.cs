

using Ecomm.Models;

namespace Ecomm.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        bool ProductExistsWithSlug(string slug, int excludeProductId = 0);
    }
}
