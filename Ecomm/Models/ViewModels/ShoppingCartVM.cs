using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ecomm.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public OrderHeader OrderHeader { get; set; }
        [ValidateNever]
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        [ValidateNever]
        public decimal Total_Price { get; set; }
        [ValidateNever]
        public int Quantity { get; set; }

    }
}
