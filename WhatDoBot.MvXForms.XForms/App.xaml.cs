using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using MvvmCross.Forms.Platform;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;


namespace WhatDoBot.MvXForms.XForms
{
    public partial class App : MvxFormsApplication
    {
        public static void StartAppCenter()
        {
            AppCenter.Start
            (
                "ios=new seecret;"
                + "uwp={Your UWP App secret here};"
                + "android={Your Android App secret here}",
                typeof(Analytics),
                typeof(Crashes)
            );
        }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}