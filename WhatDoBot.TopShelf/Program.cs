using System;
using System.IO;
using System.Reflection;
using Topshelf;
using Noobot.Core;
using Noobot.Core.Configuration;

namespace WhatDoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Console.WriteLine($"Noobot.Core assembly version: {Assembly.GetAssembly(typeof(INoobotCore)).GetName().Version}");

            HostFactory.Run(x =>
            {
                var cctx = new ModelContext();
                var reader = new DbConfigReader(cctx);
                x.AddCommandLineDefinition("botKey", reader.SetBotKey);
                reader.SetBotKey("xoxb-331875186982-ebTNK7VU20R0z6QuilFlB9IV");

                //x.AfterInstall(reader.InstallConfig);
                //x.BeforeUninstall(reader.PurgeConfig);

                x.Service<WhatDoNoobotHost>(s =>
                {
                    s.ConstructUsing(name => );

                    s.WhenStarted(n =>
                    {
                        
                        cctx.Database.EnsureCreated();
                        n.Start();
                    });

                    s.WhenStopped(n => n.Stop());
                });

                x.RunAsNetworkService();
                x.SetDisplayName("Noobot");
                x.SetServiceName("Noobot");
                x.SetDescription("An extensible Slackbot built in C#");
            });
        }
    }
}
