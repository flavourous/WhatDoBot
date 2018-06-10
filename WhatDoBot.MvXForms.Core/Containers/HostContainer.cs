using LetsAgree.IOC.Extensions.MvxSimpleShim;
using LetsAgree.IOC.MvxSimpleShim;

namespace WhatDoBot.MvXForms.Core
{
    public class HostContainer : IHostContainer
    {
        public IWhatDoNoobotHost Host { get; }
        public HostContainer(IConfigContainer config, CreatableTypesContainer ch)
        {
            Host = new WhatDoNoobotHost
                <IMvxSimpleDecoratingConfig,
                IMvxSimpleConfig,
                IMvxSimpleDecoratingRegistry,
                IMvxSimpleContainer>
                (config.Reader, () => new MvxSimpleIocWithDecoration(ch.CreatableTypes));
        }
    }
}
