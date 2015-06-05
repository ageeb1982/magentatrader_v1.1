using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentaTrader.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string AspNetUserId { get; set; }
        
        // Variables for PackageLedger
        public int SaleId { get; set; }
        public string SalesNumber { get; set; }
        public string SalesDate { get; set; }
        public string RenewalDate { get; set; }
        public string ExpiryDate { get; set; }
        public string Particulars { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public bool IsRefunded { get; set; }
        public int ProductPackageId { get; set; }
        public string ProductPackage { get; set; }
        public string ProductPackageURL { get; set; }

        // Variables for Roles
        public string Roles { get; set; }
    }
}