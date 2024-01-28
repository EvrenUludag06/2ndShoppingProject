using System.Collections;
using System.Collections.Generic;

namespace AlobarShopp.Models
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}