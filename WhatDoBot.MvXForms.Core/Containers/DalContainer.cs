using WhatDoBot.MvXForms.Core.ViewModels;

namespace WhatDoBot.MvXForms.Core
{
    public class DalContainer : IDalContainer
    {
        public ModelContext Context { get; }
        public DalContainer(IPlatformConfigurationService pcs)
        {
            Context = new ModelContext(pcs.UserDataLocation);
            Context.Database.EnsureCreated();
        }
    }
}
