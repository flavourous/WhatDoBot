using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatDoBot.MvXForms.Core.ViewModels
{
    public class MainPageViewModel : MvxViewModel
    {
        private readonly WhatDoNoobotHost host;
        private readonly IMvxNavigationService navigationService;
        public MainPageViewModel(IMvxNavigationService navigationService)
        {
            this.navigationService = navigationService;

            Mvx.RegisterType<IMvxCommandHelper, MvxStrongCommandHelper>();

            var dal = new ModelContext();
            dal.Database.EnsureCreated();
            var configReader = new DbConfigReader(dal);
            configReader.SetBotKey("xoxb-331875186982-ebTNK7VU20R0z6QuilFlB9IV"); // hack for noow
            host = new WhatDoNoobotHost(configReader);

            Users = dal.Users.Select(x => new UserViewModel(x, dal)).ToList();
            ViewUserCommand = new MvxAsyncCommand<UserViewModel>(async g => await navigationService.Navigate(g));

            StartBot = new MvxAsyncCommand(DoStartBot, () => CanStartBot) { ShouldAlwaysRaiseCECOnUserInterfaceThread = true };
            StopBot = new MvxAsyncCommand(DoStopBot, () => CanStopBot) { ShouldAlwaysRaiseCECOnUserInterfaceThread = true };
            SetBotRun(false, "Not started");
        }

        async Task DoStartBot()
        {
            await Task.Run(async () =>
            {
                SetBotRun(null, "Starting bot");
                try
                {
                    await host.Start();
                    SetBotRun(true, "Bot running");
                }
                catch (Exception e)
                {
                    SetBotRun(false, "Error starting: " + e.Message);
                }
            });
        }
        async Task DoStopBot()
        {
            await Task.Run(() =>
            {
                SetBotRun(null, "Stopping bot");
                host.Stop();
                SetBotRun(false, "Stopped");
            });
        }

        void SetBotRun(bool? stat, String stats)
        {
            InvokeOnMainThread(() =>
            {
                BotStatus = stats;
                CanStartBot = !(stat ?? true);
                CanStopBot = (stat ?? false);
                var ch = Mvx.Resolve<IMvxCommandHelper>();
                ch.RaiseCanExecuteChanged(StopBot);
                ch.RaiseCanExecuteChanged(StartBot);
            });
        }

        public IMvxAsyncCommand<UserViewModel> ViewUserCommand { get; set; }
        public IList<UserViewModel> Users { get; set; } = new MvxObservableCollection<UserViewModel>();

        String botStatus;
        public String BotStatus { get => botStatus; set => this.RaiseAndSetIfChanged(ref botStatus, value); }

        public IMvxAsyncCommand StartBot { get; set; }
        bool canStartBot = true;
        public bool CanStartBot { get => canStartBot; set => this.RaiseAndSetIfChanged(ref canStartBot, value); }

        public IMvxAsyncCommand StopBot { get; set; }
        bool canStopBot = true;
        public bool CanStopBot { get => canStopBot; set => this.RaiseAndSetIfChanged(ref canStopBot, value); }
    }
}
