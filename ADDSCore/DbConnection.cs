﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using ADDSCore.Model;

namespace ADDSCore
{
    class DbConnection:IDisposable
    {
        public ACSListContext db { get; }
        public DbConnection()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<ACSListContext>();
            var options = optionsBuilder.UseSqlServer(connectionString).Options;

            db = new ACSListContext(options);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
