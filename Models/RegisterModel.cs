using System.ComponentModel.DataAnnotations;
namespace UPC_DropDown.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password!")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }

        [Compare("UserPassword", ErrorMessage = "Password doesn't match!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword {  get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
    }
}
