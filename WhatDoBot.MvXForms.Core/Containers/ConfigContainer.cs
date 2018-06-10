namespace WhatDoBot.MvXForms.Core
{
    class ConfigContainer : IConfigContainer
    {
        public IDbConfigReader Reader { get; }
        public ConfigContainer(IDalContainer dal) => Reader = new DbConfigReader(dal.Context);
    }
}
