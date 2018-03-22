using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class ModelContext : DbContext
    {
        public DbSet<LogModel> Logs { get; set; }
        public DbSet<UserModel> Users { get; set; }

        public ModelContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=whatdo.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
