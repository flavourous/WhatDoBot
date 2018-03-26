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
                cctx.Database.EnsureCreated();
                var reader = new DbConfigReader(cctx);
                x.AddCommandLineDefinition("botKey", k =>
                {
                    reader.SetBotKey(k).Wait();
                    Console.WriteLine("Wrote BotKey to configuration table");
                });

                x.Service<WhatDoNoobotHost>(s =>
                {
                    s.ConstructUsing(name => new WhatDoNoobotHost(reader));

                    s.WhenStarted((n,c) =>
                    {
                        try
                        {
                            n.Start().Wait();
                            Console.WriteLine("NooBot Running");
                            return true;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"Failed to start NooBot{Environment.NewLine}{e.ToString()}");
                            return false;
                        }
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
