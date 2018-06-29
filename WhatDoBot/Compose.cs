using Common.Logging;
using LetsAgree.IOC;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Noobot.Core.MessagingPipeline.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public static class Compose<C, Cl, R, T> 
        where R :IGenericRegistration<C>, IGenericLocatorRegistration<Cl>, IContainerGeneration<T>
        where C : ISingletonConfig<C>, ICollectionConfig<C>, IDecoratorConfig<C>
        where Cl : ISingletonConfig<Cl>
        where T : IBasicContainer, IGenericContainer 
    {
        public static void NoobotCore(R registry)
        {
            CompositionRoot<C, Cl, R, T>.Compose(registry, r =>
            {
                r.Register<IMiddleware, WhatDoBotLogMiddleware>().AsDecorator();
            });
        }
    }
}
