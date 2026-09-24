using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadMoreBooks.Data;
using ReadMoreBooks.Models;

namespace ReadMoreBooks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===============================================================");
            Console.WriteLine("=== ReadMore Books - EF Core Conventions Assignment 01 ===");
            Console.WriteLine("===============================================================\n");

            using (var context = new BookStoreDbContext())
            {
                // Requirement 6: Demonstrate that the database is created correctly
                Console.WriteLine("[Step 1] Ensuring fresh database creation...");
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Console.WriteLine("Database 'ReadMoreBooksDB' created successfully on SQL Server!\n");

                Console.WriteLine("[Step 2] Seeding initial data by convention...");
                
                // 1. Create Authors
                var author1 = new Author
                {
                    FirstName = "Robert",
                    LastName = "Martin",
                    Email = "unclebob@cleancoder.com",
                    Biography = "Author of Clean Code and renowned software craftsmanship advocate.",
                    DateOfBirth = new DateTime(1952, 12, 5)
                };

                var author2 = new Author
                {
                    FirstName = "Andrew",
                    LastName = "Hunt",
                    Email = "andy@pragprog.com",
                    Biography = "Co-author of The Pragmatic Programmer and Agile Manifesto signatory.",
                    DateOfBirth = new DateTime(1964, 5, 12)
                };

                // 2. Create Categories
                var catSoftware = new Category
                {
                    Name = "Software Engineering",
                    Description = "Software architecture, best practices, and programming principles.",
                    IsActive = true
                };

                var catDatabase = new Category
                {
                    Name = "Databases and Cloud",
                    Description = "Relational databases, SQL optimization, and cloud storage.",
                    IsActive = true
                };

                // 3. Create Books
                var book1 = new Book
                {
                    Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    ISBN = "978-0132350884",
                    Price = 45.99m,
                    NumberOfPages = 464,
                    YearPublished = 2008,
                    IsInStock = true,
                    Author = author1,
                    Category = catSoftware
                };

                var book2 = new Book
                {
                    Title = "The Pragmatic Programmer: Your Journey To Mastery",
                    ISBN = "978-0135957059",
                    Price = 49.50m,
                    NumberOfPages = 352,
                    YearPublished = 2019,
                    IsInStock = true,
                    Author = author2,
                    Category = catSoftware
                };

                var book3 = new Book
                {
                    Title = "Clean Architecture: A Craftsman's Guide",
                    ISBN = "978-0134494166",
                    Price = 39.99m,
                    NumberOfPages = 432,
                    YearPublished = 2017,
                    IsInStock = false,
                    Author = author1,
                    Category = catSoftware
                };

                context.Authors.AddRange(author1, author2);
                context.Categories.AddRange(catSoftware, catDatabase);
                context.Books.AddRange(book1, book2, book3);
                context.SaveChanges();
                Console.WriteLine("Seeding completed successfully!\n");

                Console.WriteLine("[Step 3] Querying database using EF Core...");
                var booksFromDb = context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .ToList();

                Console.WriteLine("------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0,-4} | {1,-40} | {2,-15} | {3,-20} | {4,-6} | {5,-8}", "Id", "Title", "Author", "Category", "Price", "In Stock");
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------");

                foreach (var b in booksFromDb)
                {
                    string authorName = $"{b.Author.FirstName} {b.Author.LastName}";
                    Console.WriteLine("{0,-4} | {1,-40} | {2,-15} | {3,-20} | ${4,-5:F2} | {5,-8}", 
                        b.Id, 
                        b.Title.Length > 40 ? b.Title.Substring(0, 37) + "..." : b.Title, 
                        authorName, 
                        b.Category.Name, 
                        b.Price, 
                        b.IsInStock ? "Yes" : "No");
                }
                Console.WriteLine("------------------------------------------------------------------------------------------------------------------\n");

                Console.WriteLine("=== Assignment 01 Completed Successfully with Zero Errors! ===");
            }
        }
    }
}
