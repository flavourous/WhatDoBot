using Common.Logging;
using LetsAgree.IOC;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    public class WhatDoNoobotHost<C, Cl, R, T> : IWhatDoNoobotHost
        where R : IDynamicRegistration<C>, IGenericRegistration<C>, IScanningRegistraction<INoConfig>, IGenericLocatorRegistration<Cl>, IContainerGeneration<T>
        where C : ISingletonConfig, IDecoratorConfig
        where Cl : ISingletonConfig
        where T : IBasicContainer, IGenericContainer 
    {
        private readonly IConfigReader _configReader;
        private INoobotCore _noobotCore;
        private readonly IConfiguration _configuration;

        readonly Func<R> reg;
        public WhatDoNoobotHost(IConfigReader configReader, Func<R> reg)
        {
            this.reg = reg;
            _configReader = configReader;
            _configuration = new WhatDoBotConfig();
        }

        public async Task Start()
        {
            IContainerFactory containerFactory = new ContainerFactory<C, Cl, R, T>(_configuration, _configReader, reg, LogManager.GetLogger(GetType()));
            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();
            await _noobotCore.Connect();
        }

        public void Stop()
        {
            _noobotCore?.Disconnect();
        }
    }
}
