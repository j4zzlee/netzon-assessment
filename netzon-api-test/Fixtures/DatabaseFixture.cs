using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using netzon_assetment.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace netzon_api_test.Fixtures
{
    public class DatabaseFixture
    {
        public DatabaseFixture()
        {
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", "host=localhost;port=5432;database=netzon-assessment-db-test;username=netzon;password=n3tz0n@123");
            Environment.SetEnvironmentVariable("JWT_SECRET", "jqQtDC5qZcnOqdFl2y4ef2VpHOCF3aMKWeR2faNr3ZbfM5T7n59iPZxPTV0gqNC1");

            // ... initialize data in the test database ...

            
            var options = new DbContextOptionsBuilder<NetzonDbContext>();
            options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
            using (var dbContext = new NetzonDbContext(options.Options))
            {
                dbContext.Database.EnsureDeleted();
            }
        }

    }
}
