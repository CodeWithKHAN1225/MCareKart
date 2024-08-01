using System.ComponentModel.DataAnnotations;

namespace UPC_DropDown.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public decimal Salary { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
