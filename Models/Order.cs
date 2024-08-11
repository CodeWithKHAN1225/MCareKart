using System;
using System.Collections.Generic;

namespace UPC_DropDown.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
    public class PaymentViewModel
    {
        public List<OrderItemViewModel> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public List<int> OrderIds { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class PlaceOrderModel
    {
        public string PaymentOption { get; set; }
        public List<int> OrderIds { get; set; }
    }

}
