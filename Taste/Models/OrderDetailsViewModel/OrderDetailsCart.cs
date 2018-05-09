using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taste.Models.OrderDetailsViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
