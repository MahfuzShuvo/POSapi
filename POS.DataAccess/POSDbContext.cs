using Microsoft.EntityFrameworkCore;
using POS.Common.Models;
using POS.Common.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.DataAccess
{
    public class POSDbContext : DbContext
    {
        public POSDbContext(DbContextOptions<POSDbContext> options) : base(options)
        {

        }

        public virtual DbSet<UserSession> UserSession { get; set; }
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountStatement> AccountStatement { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<PaymentStatus> PaymentStatuse { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<PurchaseProductMapping> PurchaseProductMapping { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<RolePermissionMapping> RolePermissionMapping { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<SystemUser> SystemUser { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<VMProduct> VMProduct { get; set; }
        public virtual DbSet<VMPurchase> VMPurchase { get; set; }
        public virtual DbSet<VMAccountStatement> VMAccountStatement { get; set; }
        public virtual DbSet<VMGetAccountBalanceExpense> VMGetAccountBalanceExpense { get; set; }
        public virtual DbSet<VMCountProductByCategory> VMCountProductByCategory { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(x => x.UserSessionID);
                entity.ToTable("UserSession");
            });
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(x => x.AccountID);
                entity.ToTable("Account");
            });
            modelBuilder.Entity<AccountStatement>(entity =>
            {
                entity.HasKey(x => x.AccountStatementID);
                entity.ToTable("AccountStatement");
            });
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(x => x.BrandID);
                entity.ToTable("Brand");
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.CategoryID);
                entity.ToTable("Category");
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.CustomerID);
                entity.ToTable("Customer");
            });
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(x => x.ExpenseID);
                entity.ToTable("Expense");
            });
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(x => x.InvoiceID);
                entity.ToTable("Invoice");
            });
            modelBuilder.Entity<PaymentStatus>(entity =>
            {
                entity.HasKey(x => x.PaymentStatusID);
                entity.ToTable("PaymentStatus");
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.ProductID);
                entity.ToTable("Product");
            });
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(x => x.PurchaseID);
                entity.ToTable("Purchase");
            }); 
            modelBuilder.Entity<PurchaseProductMapping>(entity =>
            {
                entity.HasKey(x => x.PurchaseProductMappingID);
                entity.ToTable("PurchaseProductMapping");
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.RoleID);
                entity.ToTable("Role");
            }); 
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(x => x.PermissionID);
                entity.ToTable("Permission");
            });
            modelBuilder.Entity<RolePermissionMapping>(entity =>
            {
                entity.HasKey(x => x.RolePermissionMappingID);
                entity.ToTable("RolePermissionMapping");
            });
            modelBuilder.Entity<Sales>(entity =>
            {
                entity.HasKey(x => x.SalesID);
                entity.ToTable("Sales");
            });
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(x => x.SupplierID);
                entity.ToTable("Supplier");
            });
            modelBuilder.Entity<SystemUser>(entity =>
            {
                entity.HasKey(x => x.SystemUserID);
                entity.ToTable("SystemUser");
            });
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(x => x.UnitID);
                entity.ToTable("Unit");
            });

            modelBuilder.Entity<VMGetAccountBalanceExpense>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("GetAccountBalanceExpense");
            });
            modelBuilder.Entity<VMProduct>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("GetAllProductView");
            });
            modelBuilder.Entity<VMPurchase>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("GetAllPurchaseView");
            });
            modelBuilder.Entity<VMAccountStatement>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("GetAccountStatement");
            });
            modelBuilder.Entity<VMCountProductByCategory>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("CountProductByCategory");
            });
        }
    }
}
