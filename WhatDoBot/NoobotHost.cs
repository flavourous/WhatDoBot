using Common.Logging;
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
    public class WhatDoNoobotHost
    {
        private readonly IConfigReader _configReader;
        private INoobotCore _noobotCore;
        private readonly IConfiguration _configuration;

        public WhatDoNoobotHost(IConfigReader configReader)
        {
            _configReader = configReader;
            _configuration = new WhatDoBotConfig();
        }

        public async Task Start()
        {
            IContainerFactory containerFactory = new ContainerFactory(_configuration, _configReader, LogManager.GetLogger(GetType()));
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
