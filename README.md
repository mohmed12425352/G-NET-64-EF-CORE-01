# National Bank Group — Bank Management System (EF Core Assignment 04)

> **Route Academy — ASP.NET Core & Entity Framework Core**  
> **Course:** G-NET-64  
> **Assignment:** EF Core Assignment 04 (Bank Management System)

---

## 🏦 Overview

The National Bank Group data layer built using **EF Core Code-First** (.NET 9.0).  
The system models national bank branches, branch managers, customers (individuals and businesses), multi-owner accounts, and account transactions, coupled with an interactive console application.

---

## 📐 ER Diagram & Domain Models

### Entities & Relationships

1. **`Branch`**
   - **PK:** `Code` (`string`, e.g. `CAI-01`, `ALX-01`)
   - Attributes: `Name`, `Address`, `PhoneNumber`, `ManagerId`
   - **1 : 1** relationship with `Manager` (Foreign Key `ManagerId` unique)
   - **1 : N** relationship with `Account`

2. **`Manager`**
   - **PK:** `Id` (`int`, Identity)
   - Attributes: `FullName`, `Email`, `PhoneNumber`, `HireDate`
   - **1 : 1** navigation to `Branch`

3. **`Customer`**
   - **PK:** `Id` (`int`, Identity)
   - Attributes: `FullName`, `NationalId`, `DateOfBirth`, `Email`, `PhoneNumber`, `Address`, `CustomerType` (`Individual` / `Business`)
   - **N : M** relationship with `Account` through `CustomerAccount`

4. **`Account`**
   - **PK:** `AccountNumber` (`string`, e.g. `2001-CUR`, `3000-BUS`)
   - Attributes: `AccountType` (`Savings`, `Current`, `Business`), `CurrentBalance` (`decimal(18,2)`), `OpeningDate`, `BranchCode`
   - **N : 1** relationship with `Branch`
   - **1 : N** relationship with `Transaction`
   - **N : M** relationship with `Customer` through `CustomerAccount`

5. **`CustomerAccount` (Junction Table with Payload)**
   - **Composite PK:** `(CustomerId, AccountNumber)`
   - Payload attributes:
     - `OwnershipStartDate` (`DateTime`)
     - `OwnershipType` (`Primary` / `CoHolder`)
     - `AccountStatus` (`Active` / `Closed`)

6. **`Transaction`**
   - **PK:** `TransactionNumber` (`int`, Identity)
   - Attributes: `TransactionDate`, `Amount` (`decimal(18,2)`), `TransactionType` (`Deposit`, `Withdrawal`, `Payment`, etc.), `Note`, `AccountNumber`
   - **N : 1** relationship with `Account`

---

## 🚀 Features & Interactive Menu

The application features a resilient console menu interface:

```text
=========================================
  National Bank — Management
=========================================
  1) Add a new Customer
  2) Open a new Account for a Customer
  3) Update Account Status (Active / Closed)
  4) Remove an Account from a Customer
  5) List all Customers (with accounts)
  0) Exit
-----------------------------------------
```

### Functional Highlights:
1. **Option 1 - Add Customer**: Validates inputs, handles full details and Customer Type (Individual / Business).
2. **Option 2 - Open Account**: Validates branch code & customer ID existence, creates account and links as Primary or Co-Holder.
3. **Option 3 - Update Account Status**: Updates status between Active and Closed for specified customer and account.
4. **Option 4 - Remove Account**: Unlinks customer from account; automatically purges the account if no other owners remain.
5. **Option 5 - List All Customers**: Clean, formatted display showing each customer and all their associated accounts, balances, roles, statuses, and branch names.

---

## 🛠️ Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- Microsoft SQL Server / LocalDB

### Database Setup & Run
```bash
# Clone the repository
git clone https://github.com/mohmed12425352/G-NET-64-EF-04.git
cd G-NET-64-EF-04

# Run migrations (or the app will automatically migrate/seed on first start)
dotnet ef database update

# Run the console application
dotnet run
```
