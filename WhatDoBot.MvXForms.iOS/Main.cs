using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.iOS;
using MvvmCross.Forms.Platform;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform.Logging;
using MvvmCross.Platform.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIKit;

namespace WhatDoBot.MvXForms.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
    public class LinkerPleaseInclude
    {
        public void Include(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
    }

    public class Setup : MvxFormsIosSetup
    {
        public Setup(IMvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        //protected override void PerformBootstrapActions()
        //{
        //    base.PerformBootstrapActions();

        //    PluginLoader.Instance.EnsureLoaded();
        //}


        protected override MvxFormsApplication CreateFormsApplication()
        {
            return new XForms.App();
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }

        protected override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            foreach (var s in base.GetViewModelAssemblies())
                yield return s;
            yield return typeof(Core.ViewModels.MainPageViewModel).Assembly;
        }

        protected override IEnumerable<Assembly> GetViewAssemblies()
        {
            foreach (var b in base.GetViewAssemblies())
                yield return b;
            yield return typeof(XForms.MainPage).Assembly;
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return base.CreateDebugTrace();
        }
        protected override IMvxLogProvider CreateLogProvider()
        {
            return base.CreateLogProvider();
        }
    }
}