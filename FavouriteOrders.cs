using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10274934_PRG2Assignment
{
    // A favourite order stores only the OrderID of an existing DELIVERED order.
    // This makes the feature very simple:
    // - Customer favourites an old delivered order
    // - When re-ordering, we find that order and copy its items to a new order
    public class FavouriteOrder
    {
        public string CustomerEmail { get; set; }
        public int OrderId { get; set; }
        public string FavouriteName { get; set; }

        public FavouriteOrder(string email, int orderId, string favName)
        {
            CustomerEmail = email;
            OrderId = orderId;
            FavouriteName = favName;
        }
    }

}
