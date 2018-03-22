using Noobot.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatDoBot
{
    class JsonConfigOverride : IConfigReader
    {
        readonly string configFile;
        readonly string key;
        JsonConfigReader jcr;
        public JsonConfigOverride(string key)
        {
            // use json fike iof tgere, use key if supplied.
            this.key = key;
            // use current WD - could use AppData, not sure.
            this.configFile = "whatDo.config";

            if (File.Exists(configFile))
                jcr = new JsonConfigReader(configFile);
        }

        public void InstallConfig()
        {
            // move config to file - just do it
            File.WriteAllText(configFile, "{ \"slack:apiToken\": \"" + (key ?? "") + "\" }");
            jcr = new JsonConfigReader(configFile);
        }

        public void PurgeConfig()
        {
            // remove any config
            File.Delete(configFile);
        }

        public string SlackApiKey => jcr?.SlackApiKey ?? key;
        public bool HelpEnabled => jcr?.HelpEnabled ?? true;
        public bool StatsEnabled => jcr?.StatsEnabled ?? true;
        public bool AboutEnabled => jcr?.AboutEnabled ?? true;
        public T GetConfigEntry<T>(string entryName)
        {
            if (jcr == null) return default(T);
            return jcr.GetConfigEntry<T>(entryName);
        }
    }
}
