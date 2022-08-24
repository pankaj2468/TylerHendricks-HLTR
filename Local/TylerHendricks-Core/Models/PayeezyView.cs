using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class PayeezyView
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Amount { get; set; }
        public string SequenceNumber { get; set; }
        public string TimeStamp { get; set; }
        public string Hash { get; set; }
        public string MerchantCookie1 { get; set; }
        public string MerchantCookie2 { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentGateway { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }
        public string PatientId { get; set; }
        private List<string> products;
        public List<string> GetProducts()
        {
            return products;
        }
        public void SetProducts(List<string> value)
        {
            products = value;
        }
    }
}
