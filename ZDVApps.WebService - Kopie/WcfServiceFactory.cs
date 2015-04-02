using Microsoft.Practices.Unity;
using Unity.Wcf;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Contracts.WebServices;
using ZDVApps.DataTransfer.Contracts;
using ZDVApps.Services;

namespace ZDVApps.WebService
{
	public class WcfServiceFactory : UnityServiceHostFactory
    {
        protected override void ConfigureContainer(IUnityContainer container)
        {
            DependencyConfig.RegisterDependencies(container);
			// register all your components with the container here
            container.
                RegisterType<ISynchronizationWebService, SynchronizationWebService>(new HierarchicalLifetimeManager()).
                RegisterType<ISynchronizationService, SynchronizationService>(new HierarchicalLifetimeManager()).
                RegisterType<IAdminService,AdminService>(new HierarchicalLifetimeManager()).
                RegisterType<IAdminWebService, AdminWebService>(new HierarchicalLifetimeManager()).
                RegisterType<IZDVApps, ZDVApps>(new HierarchicalLifetimeManager());
            
            //.RegisterType<DataContext>(new HierarchicalLifetimeManager());
        }
    }    
}