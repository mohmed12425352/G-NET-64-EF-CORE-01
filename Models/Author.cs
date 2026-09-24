using System;
using System.Collections.Generic;

namespace ReadMoreBooks.Models
{
    // Author Entity - Defined using EF Core Conventions Only
    public class Author
    {
        // By Convention: 'Id' property is automatically treated as Primary Key (Identity column)
        public int Id { get; set; }
        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        // Navigation Property: One Author has Many Books (1-to-many relationship)
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
