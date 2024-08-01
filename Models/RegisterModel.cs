using System.ComponentModel.DataAnnotations;
namespace UPC_DropDown.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password!")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string UserEmail { get; set; }
    }
}
