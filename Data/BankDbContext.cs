using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Models;

namespace BankManagementSystem.Data
{
    public class BankDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Manager> Managers { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;

        public BankDbContext()
        {
        }

        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=NationalBankDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Branch Configuration
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Code);
                entity.Property(b => b.Code).HasMaxLength(50);
                entity.Property(b => b.Name).HasMaxLength(150).IsRequired();
                entity.Property(b => b.Address).HasMaxLength(250);
                entity.Property(b => b.PhoneNumber).HasMaxLength(50);

                // 1:1 with Manager
                entity.HasOne(b => b.Manager)
                      .WithOne(m => m.Branch)
                      .HasForeignKey<Branch>(b => b.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 2. Manager Configuration
            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.FullName).HasMaxLength(150).IsRequired();
                entity.Property(m => m.Email).HasMaxLength(150);
                entity.Property(m => m.PhoneNumber).HasMaxLength(50);
            });

            // 3. Customer Configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FullName).HasMaxLength(150).IsRequired();
                entity.Property(c => c.NationalId).HasMaxLength(50).IsRequired();
                entity.Property(c => c.Email).HasMaxLength(150);
                entity.Property(c => c.PhoneNumber).HasMaxLength(50);
                entity.Property(c => c.Address).HasMaxLength(250);
                entity.Property(c => c.CustomerType).HasMaxLength(50).IsRequired();
            });

            // 4. Account Configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountNumber);
                entity.Property(a => a.AccountNumber).HasMaxLength(50);
                entity.Property(a => a.AccountType).HasMaxLength(50).IsRequired();
                entity.Property(a => a.CurrentBalance).HasColumnType("decimal(18,2)");

                entity.HasOne(a => a.Branch)
                      .WithMany(b => b.Accounts)
                      .HasForeignKey(a => a.BranchCode)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 5. CustomerAccount (Junction Table with Payload)
            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

                entity.Property(ca => ca.OwnershipType).HasMaxLength(50).IsRequired();
                entity.Property(ca => ca.AccountStatus).HasMaxLength(50).IsRequired();

                entity.HasOne(ca => ca.Customer)
                      .WithMany(c => c.CustomerAccounts)
                      .HasForeignKey(ca => ca.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ca => ca.Account)
                      .WithMany(a => a.CustomerAccounts)
                      .HasForeignKey(ca => ca.AccountNumber)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 6. Transaction Configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionNumber);
                entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
                entity.Property(t => t.TransactionType).HasMaxLength(50).IsRequired();
                entity.Property(t => t.Note).HasMaxLength(250);

                entity.HasOne(t => t.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AccountNumber)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
