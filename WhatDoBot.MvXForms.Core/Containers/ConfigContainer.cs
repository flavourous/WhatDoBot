using Noobot.Core.Configuration;

namespace WhatDoBot.MvXForms.Core
{
    class ConfigContainer : IConfigContainer, IConfigReader
    {
        public IDbConfigReader Reader { get; }
        public ConfigContainer(IDalContainer dal) => Reader = new DbConfigReader(dal.Context);

        // Proxy because IDBConfigReader is not easily constructable by ioc
        public string SlackApiKey => Reader.SlackApiKey;
        public bool HelpEnabled => Reader.HelpEnabled;
        public bool StatsEnabled => Reader.StatsEnabled;
        public bool AboutEnabled => Reader.AboutEnabled;
        public T GetConfigEntry<T>(string entryName) => Reader.GetConfigEntry<T>(entryName);
    }
}
