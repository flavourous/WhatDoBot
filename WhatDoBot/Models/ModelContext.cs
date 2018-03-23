using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class ConfigModel
    {
        [Key]
        public String Id { get; set; }
        public String Value { get; set; }
    }
    public class ModelContext : DbContext
    {
        public DbSet<LogModel> Logs { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ConfigModel> Config { get; set; }

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
