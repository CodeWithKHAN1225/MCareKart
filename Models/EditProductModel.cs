using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UPC_DropDown.Models
{
    public class EditProductModel
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100, ErrorMessage = "Product Name must not exceed 100 characters.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000.")]
        public int ProductPrice { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        public Category Category { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}