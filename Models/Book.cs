namespace ReadMoreBooks.Models
{
    // Book Entity - Defined using EF Core Conventions Only
    public class Book
    {
        // By Convention: 'Id' is Primary Key
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int NumberOfPages { get; set; }
        public int YearPublished { get; set; }
        public bool IsInStock { get; set; }

        // By Convention: Foreign Key to Author (Author + Id)
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        // By Convention: Foreign Key to Category (Category + Id)
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
