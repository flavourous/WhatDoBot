using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System.Linq;
using MvvmCross.Platform.IoC;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WhatDoBot.MvXForms.Core.ViewModels;

namespace WhatDoBot.MvXForms.Core
{
    public class MyNavigationAppStart : IMvxAppStart
    {
        protected readonly IMvxNavigationService NavigationService;

        public MyNavigationAppStart(IMvxNavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public void Start(object hint = null)
        {
            // in defulat implimentation we're not observing any errors :/
            NavigationService.Navigate<MainPageViewModel>().Wait();
        }
    }
    public class App : MvxApplication
    {
        readonly Assembly[] extra;
        public App(params Assembly[] extra)
        {
            this.extra = extra;
        }
        public override void Initialize()
        {
            CreatableTypes()
                .Concat(extra.SelectMany(CreatableTypes))
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterCustomAppStart<MyNavigationAppStart>();
        }
    }
}
