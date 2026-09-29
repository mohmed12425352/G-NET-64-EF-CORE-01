using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Models;

namespace BankManagementSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BankDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch
            {
                context.Database.EnsureCreated();
            }

            if (context.Branches.Any())
            {
                return; // Database has already been seeded
            }

            // 1. Seed Managers
            var manager1 = new Manager
            {
                FullName = "Tarek Hassan",
                Email = "tarek.hassan@nationalbank.com",
                PhoneNumber = "01001122334",
                HireDate = new DateTime(2015, 1, 1)
            };

            var manager2 = new Manager
            {
                FullName = "Sara Ibrahim",
                Email = "sara.ibrahim@nationalbank.com",
                PhoneNumber = "01112233445",
                HireDate = new DateTime(2018, 5, 15)
            };

            context.Managers.AddRange(manager1, manager2);
            context.SaveChanges();

            // 2. Seed Branches
            var branchCairo = new Branch
            {
                Code = "CAI-01",
                Name = "Cairo Main Branch",
                Address = "15 Tahrir Square, Cairo",
                PhoneNumber = "0225789000",
                ManagerId = manager1.Id
            };

            var branchAlex = new Branch
            {
                Code = "ALX-01",
                Name = "Alexandria Branch",
                Address = "22 El Horreya Road, Alexandria",
                PhoneNumber = "033987000",
                ManagerId = manager2.Id
            };

            context.Branches.AddRange(branchCairo, branchAlex);
            context.SaveChanges();

            // 3. Seed Customers
            var customer1 = new Customer
            {
                FullName = "Ahmed Ali",
                NationalId = "28501011234567",
                DateOfBirth = new DateTime(1985, 1, 1),
                Email = "ahmed.ali@email.com",
                PhoneNumber = "01012345678",
                Address = "10 Tahrir St, Cairo",
                CustomerType = "Individual"
            };

            var customer2 = new Customer
            {
                FullName = "Nile Trading LLC",
                NationalId = "10020030040050",
                DateOfBirth = new DateTime(2010, 6, 1),
                Email = "contact@niletrading.com",
                PhoneNumber = "01234567890",
                Address = "5 Corniche, Alexandria",
                CustomerType = "Business"
            };

            context.Customers.AddRange(customer1, customer2);
            context.SaveChanges();

            // 4. Seed Accounts
            var account1 = new Account
            {
                AccountNumber = "2001-CUR",
                AccountType = "Current",
                CurrentBalance = 15400.00m,
                OpeningDate = new DateTime(2021, 1, 10),
                BranchCode = "CAI-01"
            };

            var account2 = new Account
            {
                AccountNumber = "3000-BUS",
                AccountType = "Business",
                CurrentBalance = 120000.00m,
                OpeningDate = new DateTime(2022, 3, 15),
                BranchCode = "ALX-01"
            };

            context.Accounts.AddRange(account1, account2);
            context.SaveChanges();

            // 5. Seed CustomerAccount Links
            var ca1 = new CustomerAccount
            {
                CustomerId = customer1.Id,
                AccountNumber = account1.AccountNumber,
                OwnershipStartDate = new DateTime(2021, 1, 10),
                OwnershipType = "Primary",
                AccountStatus = "Active"
            };

            var ca2 = new CustomerAccount
            {
                CustomerId = customer2.Id,
                AccountNumber = account2.AccountNumber,
                OwnershipStartDate = new DateTime(2022, 3, 15),
                OwnershipType = "Primary",
                AccountStatus = "Active"
            };

            context.CustomerAccounts.AddRange(ca1, ca2);
            context.SaveChanges();
        }
    }
}
