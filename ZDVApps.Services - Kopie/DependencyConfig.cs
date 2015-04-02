using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Contracts;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Decorators;
using ZDVApps.DataAccess;
using ZDVApps.DataTransfer.Contracts;
using ZDVApps.Model.DB;
using ZDVApps.Services.DBRepositories;
using ZDVApps.Services.Decorators;
using ZDVApps.Services.UnityExtensions;

namespace ZDVApps.Services
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies(IUnityContainer container)
        {
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager()).
                RegisterType<IRepository<Appv5Package>, Appv5PackagesRepository>(new HierarchicalLifetimeManager())
                .RegisterType<IDbContext, ZdvAppsContext>(new HierarchicalLifetimeManager())
                .RegisterType(typeof (ICacheProvider), typeof (CacheProvider), new HierarchicalLifetimeManager())
                .RegisterType(typeof (IRepository<Category>), typeof (Appv5CategoryRepository),
                    new HierarchicalLifetimeManager()).
                RegisterType(typeof (IRepository<Appv5Package>), typeof (Appv5PackagesRepository),
                    new HierarchicalLifetimeManager()).
                RegisterType(typeof (IRepository<Appv5Application>), typeof (Appv5ApplicationRepository),
                    new HierarchicalLifetimeManager()).
                RegisterType<ISynchronizationService, SynchronizationService>(new HierarchicalLifetimeManager()).
                RegisterType<IAppVService, AppVService>(new HierarchicalLifetimeManager()).
                //Add an extension to build a dependent injected decorator
                AddExtension(new DecoratorContainerExtension()).
                RegisterType<IAppVRepository, AppVCache>(new HierarchicalLifetimeManager()).
                RegisterType<IAppVRepository, AppVRepository>(new HierarchicalLifetimeManager());

                    container.AddExtension(new DecoratorContainerExtension()).RegisterType<IActiveDirectoryRepository,ActiveDirectoryRepositoryCache>(new HierarchicalLifetimeManager()).
                        RegisterType<IActiveDirectoryRepository,ActiveDirectoryRepositoryDecorator>(new HierarchicalLifetimeManager());
            //container.RegisterType(typeof(AppVRepositoryDecorator), typeof(AppVCache), new HierarchicalLifetimeManager());

            //container.RegisterType(typeof(IAppVRepository), typeof(AppVRepository), new HierarchicalLifetimeManager());

        }
    }
}
