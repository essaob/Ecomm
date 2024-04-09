

using Ecomm.Models;

namespace Ecomm.Repository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderdetail);
    }
}
