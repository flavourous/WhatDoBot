using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System.Linq;
using System.IoC.Extensions;
using MvvmCross.Platform.IoC;
using System.Reflection;
using System.Text;
using WhatDoBot.MvXForms.Core.ViewModels;
using Noobot.Core.Configuration;
using System.IoC.Extensions.MvxSimpleShim;
using System.IoC.MvxSimpleShim;
using Noobot.Core.Logging;
using Common.Logging;
using SlackConnector;
using Noobot.Core.Plugins;

namespace WhatDoBot.MvXForms.Core
{
    public class MyNavigationAppStart : IMvxAppStart
    {
        protected readonly IMvxNavigationService NavigationService;

        public MyNavigationAppStart(IMvxNavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public void Start(object hint = null)
        {
            // in defulat implimentation we're not observing any errors :/
            NavigationService.Navigate<MainPageViewModel>().Wait();
        }
    }
    public class App : MvxApplication
    {
        readonly Assembly[] extra;
        public App(params Assembly[] extra)
        {
            this.extra = extra;
        }
        public override void Initialize()
        {
            CreatableTypes()
                .Concat(extra.SelectMany(CreatableTypes))
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.LazyConstructAndRegisterSingleton<IDalContainer, DalContainer>();
            Mvx.LazyConstructAndRegisterSingleton<IConfigReader, ConfigContainer>();
            Mvx.LazyConstructAndRegisterSingleton<IConfigContainer, ConfigContainer>();
            Mvx.LazyConstructAndRegisterSingleton<ILog, EmptyLogger>();
//            Mvx.LazyConstructAndRegisterSingleton<ISlackConnector, FakeSlack>();

            // ModelContext is injected on a IMiddleware and has a not-so-DI friendly constructor.
            Mvx.LazyConstructAndRegisterSingleton(() => Mvx.Resolve<IDalContainer>().Context);

            using (var ioc = new MvxSimpleIocImprovedCreator(CreatableTypes))
            {
                Compose<IMvxImprovedConfig,
                        IMvxImprovedLocatorConfig,
                        IMvxImprovedRegistry,
                        IMvxSimpleContainer>
                        .NoobotCore(ioc.Registry);
            }

            RegisterCustomAppStart<MyNavigationAppStart>();
        }
    }
}
