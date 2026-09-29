using System;

namespace BankManagementSystem.Models
{
    public class Manager
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        // Navigation property (1:1 with Branch)
        public Branch? Branch { get; set; }
    }
}
