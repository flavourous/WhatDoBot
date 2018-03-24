using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace WhatDoBot.MvXForms.Core.ViewModels
{
    public class WhatDoCommand : MvxNotifyPropertyChanged, IMvxCommand
    {
        public event EventHandler CanExecuteChanged = delegate { };
        bool executable = true;
        public bool Executable
        {
            get => executable;
            set
            {
                this.RaiseAndSetIfChanged(ref executable, value);
                RaiseCanExecuteChanged();
            }
        }

        readonly Func<object, Task> act;
        public WhatDoCommand(Func<object, Task> act) => this.act = act;
        public bool CanExecute() => executable;
        public bool CanExecute(object parameter) => executable;
        public void Execute() => Execute(null);
        public async void Execute(object parameter)
        {
            await Task.Run(async () => await act(parameter));
        }
        public void RaiseCanExecuteChanged() => CanExecuteChanged(this, new EventArgs());
    }

    public class MainPageViewModel : MvxViewModel
    {
        private readonly ModelContext dal;
        private readonly WhatDoNoobotHost host;
        private readonly IMvxNavigationService navigationService;
        public MainPageViewModel(IMvxNavigationService navigationService)
        {
            dal = new ModelContext();
            this.navigationService = navigationService;
            dal.Database.EnsureCreated();
            var configReader = new DbConfigReader(dal);
            SetBotKey = new WhatDoCommand(async k => await configReader.SetBotKey(k as String));
            host = new WhatDoNoobotHost(configReader);
            ModelContext.Changed += GetUsers; GetUsers();
            ViewUserCommand = new MvxAsyncCommand<UserViewModel>(async g => await navigationService.Navigate(g));

            StartBot = new WhatDoCommand(async o =>
            {
                setBotRun(null, "Starting bot");
                try
                {
                    await host.Start();
                    setBotRun(true, "Bot running");
                }
                catch (Exception e)
                {
                    setBotRun(false, "Error starting: " + e.Message);
                }
            });
            StopBot = new WhatDoCommand(async o =>
            {
                setBotRun(null, "Stopping bot");
                try
                {
                    host.Stop();
                    setBotRun(false, "Stopped");
                }
                catch (Exception e)
                {
                    setBotRun(false, "Error stopping: " + e.Message);
                }
            });
            setBotRun(false, "Not started");
        }

        void GetUsers()
        {
            Users = dal.Users
                       .ToArray()
                       .Select(x => new UserViewModel(x, dal))
                       .ToArray();
        }

        void setBotRun(bool? stat, String stats)
        {
            InvokeOnMainThread(() =>
            {
                BotStatus = stats;
                StartBot.Executable = !(stat ?? true);
                StopBot.Executable = (stat ?? false);
                var ch = Mvx.Resolve<IMvxCommandHelper>();
                ch.RaiseCanExecuteChanged(StopBot);
                ch.RaiseCanExecuteChanged(StartBot);
            });
        }

        public WhatDoCommand SetBotKey { get; set; }

        public IMvxAsyncCommand<UserViewModel> ViewUserCommand { get; set; }
        IEnumerable<UserViewModel> users;
        public IEnumerable<UserViewModel> Users { get => users; set => this.RaiseAndSetIfChanged(ref users, value); }

        String botStatus;
        public String BotStatus { get => botStatus; set => this.RaiseAndSetIfChanged(ref botStatus, value); }
        public WhatDoCommand StartBot { get; set; }
        public WhatDoCommand StopBot { get; set; }
    }
}
