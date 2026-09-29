using System;

namespace BankManagementSystem.Models
{
    public class Transaction
    {
        public int TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? Note { get; set; }

        // Foreign Key to Account
        public string AccountNumber { get; set; } = string.Empty;
        public Account Account { get; set; } = null!;
    }
}
