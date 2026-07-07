using EshopperMCV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EShopperMCV.Models
{
    public class Cart
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        // Constructor
        public Cart(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
