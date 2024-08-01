using System.ComponentModel.DataAnnotations;

namespace UPC_DropDown.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string CategoryStatus { get; set; }
    }
}
