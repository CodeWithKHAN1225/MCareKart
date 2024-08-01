using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UPC_DropDown.Models;
public class CartItem
{
    [Key]
    public int CartId { get; set; }

    [Required]
    public int ProductID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int Quantity { get; set; }

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; }

    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; }

    [NotMapped]
    public decimal ProductPrice => Product.ProductPrice;

    [NotMapped]
    public decimal FinalPrice => Product.ProductPrice * Quantity;
}
