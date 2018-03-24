using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatDoBot.MvXForms.Core.ViewModels
{
    public class UserViewModel : MvxViewModel
    {
        readonly ModelContext dal;
        public readonly UserModel user;
        public UserViewModel(UserModel user, ModelContext dal)
        {
            this.dal = dal;
            this.user = user;
            verified = user.Confirmed;
            Username = $"{user.Name} ({user.Email})";
            GetLogs(); ModelContext.Changed += GetLogs;
            PropertyChanged += UserViewModel_PropertyChanged;
        }

        void GetLogs()
        {
            Logs = dal.Logs
                      .OrderByDescending(x => x.When)
                      .Take(100)
                      .Select(x => new LogViewModel(x, dal))
                      .ToArray();
        }

        private void UserViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Verified":
                    user.Confirmed = verified;
                    dal.Update(user);
                    dal.SaveChanges();
                    break;
            }
        }

        public String Username { get; set; }

        bool verified = false;
        public bool Verified { get => verified; set => this.RaiseAndSetIfChanged(ref verified, value); }
        IEnumerable<LogViewModel> logs;
        public IEnumerable<LogViewModel> Logs { get => logs; set => this.RaiseAndSetIfChanged(ref logs, value); }
    }
    public class LogViewModel
    {
        public LogViewModel(LogModel m, ModelContext ctx)
        {
            When = m.When;
            What = m.Data;
            Delete = new WhatDoCommand(async o =>
            {
                ctx.Logs.Remove(m);
                await ctx.SaveChangesAsync();
                ModelContext.OnChanged();
            });
        }
        public IMvxCommand Delete { get; set; }
        public DateTime When { get; set; }
        public String What { get; set; }
    }
}
