using System;
using System.Collections.Generic;

namespace BankManagementSystem.Models
{
    public class Account
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; // "Savings", "Current", "Business"
        public decimal CurrentBalance { get; set; }
        public DateTime OpeningDate { get; set; }

        // Foreign key to Branch
        public string BranchCode { get; set; } = string.Empty;
        public Branch Branch { get; set; } = null!;

        // Navigation properties
        public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
