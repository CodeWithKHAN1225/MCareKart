using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace UPC_DropDown.Models
{
    public class Product
    {
        internal Category? Category;

        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public int CategoryID { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } // Add this line
       // public object OrderItems { get; internal set; }

        internal static object FromSqlRaw(string v)
        {
            throw new NotImplementedException();
        }

        internal static object FromSqlRaw(string v, string keyword)
        {
            throw new NotImplementedException();
        }

    }
    public class SearchRequestModel
    {
        public string Keyword { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

};