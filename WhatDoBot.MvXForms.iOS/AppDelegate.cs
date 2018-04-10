using CoreGraphics;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.iOS;
using MvvmCross.Platform;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace WhatDoBot.MvXForms.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxFormsApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            TaskCompletionSource<bool> sent = new TaskCompletionSource<bool>();
            Crashes.FailedToSendErrorReport += (o, e) => sent.TrySetResult(false);
            Crashes.SentErrorReport += (o, e) => sent.TrySetResult(true);
            XForms.App.StartAppCenter();
            Distribute.DontCheckForUpdatesInDebug();
            AppDomain.CurrentDomain.UnhandledException += (o, e) => Console.WriteLine("Exception Raised{0}----------------{0}{1}", Environment.NewLine, e.ExceptionObject);
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            Distribute.ReleaseAvailable = OnReleaseAvailable;

            if (Crashes.HasCrashedInLastSessionAsync().Result)
            {
                var sc = new SplashController
                {
                    error = Crashes.GetLastSessionCrashReportAsync().Result.Exception.InnerException.ToString(),
                    start = StartMvvMxForms
                };
                Window.RootViewController = sc;
                Window.MakeKeyAndVisible();
                Task.Run(async () =>
                {
                    var ss = await sent.Task ? "report sent" : "failed to send report";
                    InvokeOnMainThread(() => sc.load.Text = ss);
                });
            }
            else StartMvvMxForms();
            return true;
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            var title = "Version " + releaseDetails.ShortVersion + " available!";
            var alert = UIAlertController.Create(title, releaseDetails.ReleaseNotes, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, x => Distribute.NotifyUpdateAction(UpdateAction.Update)));
            if (!releaseDetails.MandatoryUpdate)
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, x => Distribute.NotifyUpdateAction(UpdateAction.Postpone)));
            return true;
        }


        void StartMvvMxForms()
        {
            var setup = new Setup(this, Window);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            LoadApplication(setup.FormsApplication);

            Window.MakeKeyAndVisible();
        }
    }
    public class SplashController : UIViewController
    {
        public String error;
        public Action start;
        public UILabel load, sad, data;
        public UIButton ok;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Starting Up";
            View.BackgroundColor = UIColor.Purple;

            // keep the code the username UITextField
            load = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Text = "uploading crash"
            };
            sad = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Text = "(._.)"
            };
            data = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.White,
                Text = error
            };
            ok = new UIButton();
            ok.SetTitle("Start", UIControlState.Normal);
            ok.PrimaryActionTriggered += (o, e) => start();

            load.Font = UIFont.BoldSystemFontOfSize(24f);
            sad.Font = UIFont.SystemFontOfSize(24f);
            data.Font = UIFont.SystemFontOfSize(12f);

            View.AddSubview(load);
            View.AddSubview(sad);
            View.AddSubview(data);
            View.AddSubview(ok);
        }
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            nfloat h = 31.0f;
            nfloat w = View.Bounds.Width;
            load.Frame = new CGRect(10, 0, w - 20, h);
            sad.Frame = new CGRect(10, h, w - 20, h);
            data.Frame = new CGRect(0, h*2, w, View.Bounds.Height - h*3);
            ok.Frame = new CGRect(50, View.Bounds.Height - h, w - 100, h);
        }
    }
}


