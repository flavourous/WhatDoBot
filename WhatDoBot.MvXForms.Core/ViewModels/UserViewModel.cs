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
        readonly UserModel user;
        public UserViewModel(UserModel user, ModelContext dal)
        {
            this.dal = dal;
            this.user = user;
            verified = user.Confirmed;
            Logs = new MvxObservableCollection<LogViewModel>
                   (
                      dal.Logs
                      .OrderByDescending(x => x.When)
                      .Take(100)
                      .Select(x => new LogViewModel { When = x.When, What = x.Data })
                   );
            PropertyChanged += UserViewModel_PropertyChanged;
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

        bool verified = false;
        public bool Verified { get => verified; set => this.RaiseAndSetIfChanged(ref verified, value); }
        public IList<LogViewModel> Logs { get; set; }
    }
    public class LogViewModel
    {
        public DateTime When { get; set; }
        public String What { get; set; }
    }
}
