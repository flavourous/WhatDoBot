using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MvvmCross.Platform.IoC;
using UIKit;

namespace WhatDoBot.MvXForms.iOS
{
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
        public void Include(MvxPropertyInjector pij)
        {
            pij = new MvxPropertyInjector();
            pij.Inject(new object(), MvxPropertyInjectorOptions.All);
            pij.Inject(new object(), MvxPropertyInjectorOptions.MvxInject);
        }
    }
}