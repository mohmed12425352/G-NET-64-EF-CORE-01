using System;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Data;
using BankManagementSystem.Models;

namespace BankManagementSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                using var context = new BankDbContext();
                // Ensure DB is created and seeded
                DbInitializer.Initialize(context);

                bool running = true;
                while (running)
                {
                    Console.Clear();
                    PrintMenu();

                    Console.Write("Enter choice: ");
                    string? input = Console.ReadLine()?.Trim();

                    switch (input)
                    {
                        case "1":
                            AddNewCustomer(context);
                            break;
                        case "2":
                            OpenNewAccount(context);
                            break;
                        case "3":
                            UpdateAccountStatus(context);
                            break;
                        case "4":
                            RemoveAccountFromCustomer(context);
                            break;
                        case "5":
                            ListAllCustomers(context);
                            break;
                        case "0":
                            running = false;
                            Console.WriteLine("Exiting National Bank Management System. Goodbye!");
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid choice. Please select an option between 0 and 5.");
                            Console.ResetColor();
                            Pause();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"A critical error occurred: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("  National Bank — Management");
            Console.WriteLine("=========================================");
            Console.WriteLine("  1) Add a new Customer");
            Console.WriteLine("  2) Open a new Account for a Customer");
            Console.WriteLine("  3) Update Account Status (Active / Closed)");
            Console.WriteLine("  4) Remove an Account from a Customer");
            Console.WriteLine("  5) List all Customers (with accounts)");
            Console.WriteLine("  0) Exit");
            Console.WriteLine("-----------------------------------------");
        }

        private static void AddNewCustomer(BankDbContext context)
        {
            Console.WriteLine("--- Add New Customer ---");

            string fullName = ReadRequiredString("Full Name     : ");
            string nationalId = ReadRequiredString("National ID   : ");
            DateTime dob = ReadDate("Date of Birth : (yyyy-MM-dd) ");
            string email = ReadRequiredString("Email         : ");
            string phone = ReadRequiredString("Phone         : ");
            string address = ReadRequiredString("Address       : ");

            Console.WriteLine("Customer Type:");
            Console.WriteLine("    1) Individual");
            Console.WriteLine("    2) Business");
            int typeChoice = ReadIntRange("  Choice: ", 1, 2);
            string customerType = typeChoice == 1 ? "Individual" : "Business";

            var customer = new Customer
            {
                FullName = fullName,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = customerType
            };

            context.Customers.Add(customer);
            context.SaveChanges();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Customer created successfully. CustomerId = {customer.Id}");
            Console.ResetColor();

            Pause();
        }

        private static void OpenNewAccount(BankDbContext context)
        {
            Console.WriteLine("--- Open New Account ---");

            string accountNumber = ReadRequiredString("Account Number : ");

            Console.WriteLine("Account Type:");
            Console.WriteLine("    1) Savings");
            Console.WriteLine("    2) Current");
            Console.WriteLine("    3) Business");
            int typeChoice = ReadIntRange("  Choice: ", 1, 3);
            string accountType = typeChoice switch
            {
                1 => "Savings",
                2 => "Current",
                3 => "Business",
                _ => "Savings"
            };

            string branchCode = ReadRequiredString("Branch Code    : ");
            int customerId = ReadInt("Customer Id    : ");

            Console.WriteLine("Ownership Role:");
            Console.WriteLine("    1) Primary");
            Console.WriteLine("    2) CoHolder");
            int roleChoice = ReadIntRange("  Choice: ", 1, 2);
            string ownershipRole = roleChoice == 1 ? "Primary" : "CoHolder";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");
            Console.ResetColor();

            var branch = context.Branches.FirstOrDefault(b => b.Code == branchCode);
            if (branch == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Branch '{branchCode}' does not exist!");
                Console.ResetColor();
                Pause();
                return;
            }

            var customer = context.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Customer #{customerId} does not exist!");
                Console.ResetColor();
                Pause();
                return;
            }

            // Check if account already exists
            var account = context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                account = new Account
                {
                    AccountNumber = accountNumber,
                    AccountType = accountType,
                    CurrentBalance = 0m,
                    OpeningDate = DateTime.Now,
                    BranchCode = branchCode
                };
                context.Accounts.Add(account);
                context.SaveChanges();
            }

            // Check if already linked
            var existingLink = context.CustomerAccounts
                .FirstOrDefault(ca => ca.CustomerId == customerId && ca.AccountNumber == accountNumber);

            if (existingLink != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Customer #{customerId} is already linked to account '{accountNumber}'.");
                Console.ResetColor();
                Pause();
                return;
            }

            var customerAccount = new CustomerAccount
            {
                CustomerId = customerId,
                AccountNumber = accountNumber,
                OwnershipStartDate = DateTime.Now,
                OwnershipType = ownershipRole,
                AccountStatus = "Active"
            };

            context.CustomerAccounts.Add(customerAccount);
            context.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Account '{accountNumber}' created and linked to customer {customerId} as {ownershipRole} owner.");
            Console.ResetColor();

            Pause();
        }

        private static void UpdateAccountStatus(BankDbContext context)
        {
            Console.WriteLine("--- Update Account Status ---");

            string accountNumber = ReadRequiredString("Account Number : ");
            int customerId = ReadInt("Customer Id    : ");

            var link = context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: No link found between customer #{customerId} and account '{accountNumber}'.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.WriteLine("New Status:");
            Console.WriteLine("    1) Active");
            Console.WriteLine("    2) Closed");
            int choice = ReadIntRange("  Choice: ", 1, 2);
            string newStatus = choice == 1 ? "Active" : "Closed";

            link.AccountStatus = newStatus;
            context.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Status updated to {newStatus}.");
            Console.ResetColor();

            Pause();
        }

        private static void RemoveAccountFromCustomer(BankDbContext context)
        {
            Console.WriteLine("--- Remove Account From Customer ---");

            string accountNumber = ReadRequiredString("Account Number : ");
            int customerId = ReadInt("Customer Id    : ");
            Console.WriteLine();

            var link = context.CustomerAccounts
                .FirstOrDefault(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: No link found between customer #{customerId} and account '{accountNumber}'.");
                Console.ResetColor();
                Pause();
                return;
            }

            context.CustomerAccounts.Remove(link);
            context.SaveChanges();

            // Check if any owners left for this account
            bool hasOtherOwners = context.CustomerAccounts.Any(ca => ca.AccountNumber == accountNumber);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ownership link deleted.");
            if (!hasOtherOwners)
            {
                var account = context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
                if (account != null)
                {
                    context.Accounts.Remove(account);
                    context.SaveChanges();
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"    That was the last owner - account '{accountNumber}' was also removed.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"    Other owners still exist for account '{accountNumber}'.");
            }
            Console.ResetColor();

            Pause();
        }

        private static void ListAllCustomers(BankDbContext context)
        {
            Console.WriteLine("--- All Customers ---");
            Console.WriteLine();

            var customers = context.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .OrderBy(c => c.Id)
                .ToList();

            if (!customers.Any())
            {
                Console.WriteLine("No customers found.");
            }
            else
            {
                foreach (var customer in customers)
                {
                    Console.WriteLine($"#{customer.Id} {customer.FullName} ({customer.CustomerType})");

                    if (!customer.CustomerAccounts.Any())
                    {
                        Console.WriteLine("    (no accounts)");
                    }
                    else
                    {
                        foreach (var ca in customer.CustomerAccounts)
                        {
                            string branchName = ca.Account?.Branch?.Name ?? "Unknown Branch";
                            string accNum = ca.AccountNumber;
                            string accType = ca.Account?.AccountType ?? "N/A";
                            decimal balance = ca.Account?.CurrentBalance ?? 0m;
                            string role = ca.OwnershipType;
                            string status = ca.AccountStatus;

                            Console.WriteLine($"    {accNum,-10} {accType,-9} Balance: {balance,12:N2}   {role,-9} {status,-8} @ {branchName}");
                        }
                    }
                }
            }

            Console.WriteLine();
            Pause();
        }

        private static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Input cannot be empty. Please try again.");
                Console.ResetColor();
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid number. Please enter a valid integer.");
                Console.ResetColor();
            }
        }

        private static int ReadIntRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                {
                    return result;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid choice. Please enter a number between {min} and {max}.");
                Console.ResetColor();
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy", "yyyy.MM.dd" };
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                if (DateTime.TryParse(input, out date))
                {
                    return date;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid date format. Expected format: yyyy-MM-dd (e.g. 2000-01-25).");
                Console.ResetColor();
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return to the menu...");
            // Avoid blocking if standard input is redirected (e.g. during automated test)
            if (Console.IsInputRedirected)
            {
                Console.ReadLine();
            }
            else
            {
                Console.ReadKey(true);
            }
        }
    }
}
