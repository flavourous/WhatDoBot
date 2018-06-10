using System;
using System.Linq;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class DbConfigReader : IDbConfigReader
    {
        readonly ModelContext ctx;
        public DbConfigReader(ModelContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task SetBotKey(String botkey)
        {
            var exist = ctx.Config.Where(x => x.Id == "botkey").FirstOrDefault();
            if (exist != null)
            {
                exist.Value = botkey;
                ctx.Update(exist);
            }
            else ctx.Config.Add(new ConfigModel { Id = "botkey", Value = botkey });
            await ctx.SaveChangesAsync();
        }

        public string SlackApiKey => ctx.Config.Where(x => x.Id == "botkey").Select(x => x.Value).FirstOrDefault();
        public bool HelpEnabled => true;
        public bool StatsEnabled => true;
        public bool AboutEnabled => true;

        public T GetConfigEntry<T>(string entryName)
        {
            var rec = ctx.Config.SingleOrDefault(x => x.Id == entryName);
            return (T)Convert.ChangeType(rec, typeof(T));
        }
    }
}
