using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
        // I dont even care at this point.  DbSet.Local is fucking useless.
        public static event Action Changed = delegate { };
        public static void OnChanged() { Changed(); }

        public DbSet<LogModel> Logs { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ConfigModel> Config { get; set; }

        readonly String userDataLocation;
        public ModelContext(String userDataLocation)
        {
            this.userDataLocation = userDataLocation;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(userDataLocation,"whatdo.db")}");
            base.OnConfiguring(optionsBuilder);
        }

        public (LogModel[] data, DateTime start) WhatHappened(int week, UserModel user)
        {
            DateTime Start = DateTime.Today;
            if (Start.DayOfWeek == 0) Start = Start.Subtract(TimeSpan.FromDays(6));
            else Start = Start.Subtract(TimeSpan.FromDays((int)Start.DayOfWeek - 1));
            Start = Start.AddDays(week * 7);
            DateTime End = Start.AddDays(7);
            return (Logs.Where(x=>x.User.Id == user.Id)
                        .Where(x => x.When >= Start && x.When <= End)
                        .ToArray(),Start);
        }
    }
}
