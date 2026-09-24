using System.Collections.Generic;

namespace ReadMoreBooks.Models
{
    // Category Entity - Defined using EF Core Conventions Only
    public class Category
    {
        // By Convention: 'Id' property is automatically treated as Primary Key
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Navigation Property: One Category has Many Books (1-to-many relationship)
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
