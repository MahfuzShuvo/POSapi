
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Helper.AuditLog
{
    public class POSAuditLogDbContext : DbContext
    {
        public POSAuditLogDbContext()
        {

        }

        public POSAuditLogDbContext(DbContextOptions<POSAuditLogDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = DBConnection.Configuration.GetConnectionString("POSAuditLog");

            optionsBuilder.UseSqlServer(conn);
        }

        public virtual DbSet<AuditLogMain> AuditLogMain { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLogMain>(entity =>
            {
                entity.HasKey(x => x.AuditLogMainID);
                entity.ToTable("AuditLogMain");
            });
            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.HasKey(x => x.ExceptionLogID);
                entity.ToTable("ExceptionLog");
            });

        }


    }

    public static class DBConnection
    {
        private static IConfiguration config;
        public static IConfiguration Configuration
        {
            get
            {
                if (config == null)
                {
                    var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                    config = builder.Build();

                }                
                return config;
            }
        }
    }
}
