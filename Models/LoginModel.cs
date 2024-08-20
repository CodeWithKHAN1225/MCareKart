using System.ComponentModel.DataAnnotations;

namespace UPC_DropDown.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password!")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string UserPassword { get; set; }
    }
}
