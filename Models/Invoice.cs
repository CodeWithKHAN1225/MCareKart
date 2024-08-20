using System.Reflection;

namespace UPC_DropDown.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public Order Order { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string MobileNumber {get;set;}
        public string EmailId { get; set; }
    }
}
