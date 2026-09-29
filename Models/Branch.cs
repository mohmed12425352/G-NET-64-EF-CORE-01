using System;
using System.Collections.Generic;

namespace BankManagementSystem.Models
{
    public class Branch
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Foreign Key for 1:1 Manager
        public int ManagerId { get; set; }
        public Manager Manager { get; set; } = null!;

        // 1:N with Account
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
