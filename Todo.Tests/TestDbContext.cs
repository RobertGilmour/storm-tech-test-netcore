using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Todo.Data;

namespace Todo.Tests
{
    public class TestDbContext : ApplicationDbContext
    {
        public TestDbContext() : base(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(CreateInMemoryDatabase()).Options)
        {
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public override void Dispose()
        {
            Database.GetDbConnection().Dispose();
            base.Dispose();
        }
    }
}
