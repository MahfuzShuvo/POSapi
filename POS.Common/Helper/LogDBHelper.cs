using Microsoft.EntityFrameworkCore;
using POS.Common.Helper.AuditLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Common.Helper
{
    public static class LogDBHelper
    {
        private static string _connectionString;

        public static void Initialize(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {

            _connectionString = configuration.GetSection("ConnectionStrings").GetSection("AuditLogCN").Value;

        }
        public static POSAuditLogDbContext CreateDbContext()
        {

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("In appsetting json file connection string has not been initialized.");
            }

            DbContextOptionsBuilder<POSAuditLogDbContext> optionsBuilder = new DbContextOptionsBuilder<POSAuditLogDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);

            return new POSAuditLogDbContext(optionsBuilder.Options);
        }

    }
}