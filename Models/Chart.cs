using System.ComponentModel.DataAnnotations;

namespace UPC_DropDown.Models
{
    public class Chart
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserEmail { get; set; }
        public string UserExperience { get; set; }
        public int UserSalary {  get; set; }
    }
}
