using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WhatDoBot.MvXForms.Core;
using WhatDoBot.MvXForms.Core.ViewModels;

namespace WhatDoBot.MvXForms.XForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : MvxContentPage<UserViewModel>
    {
        public UserPage()
        {
            InitializeComponent();
        }
    }
}