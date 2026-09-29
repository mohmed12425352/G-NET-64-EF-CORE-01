using System;

namespace BankManagementSystem.Models
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public string AccountNumber { get; set; } = string.Empty;
        public Account Account { get; set; } = null!;

        public DateTime OwnershipStartDate { get; set; }
        public string OwnershipType { get; set; } = "Primary"; // "Primary" or "CoHolder"
        public string AccountStatus { get; set; } = "Active";  // "Active" or "Closed"
    }
}
