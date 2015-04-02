using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using ZDVApps.Common.Contracts;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Decorators;
using ZDVApps.Model.Appv;
using ZDVApps.Model.DB;
using ZDVApps.Services.AD;
using ZDVApps.Services.Properties;
using Application = ZDVApps.Dtos.Application;
using Package = ZDVApps.Dtos.Package;

namespace ZDVApps.Services
{
    public class AppVService : IAppVService
    {
        private readonly IRepository<Appv5Package> _packageRepository; 
        private readonly IAppVRepository _appVRepository;
        //private readonly Lazy<IAppVRepository> _appV;
        public AppVService(IAppVRepository appVRepository,IRepository<Appv5Package> packageRepository )
        {
            _appVRepository = appVRepository;
            _packageRepository = packageRepository;
            //_appV = new Lazy<IAppVRepository>(() => new AppVCache(new AppVRepository(Properties.Settings.Default.API_Appv)));
        }


        public IEnumerable<Package> GetAllPackages()
        {
            var packages = _appVRepository.GetAllPackages();
            var mappedPackages = new List<Package>();
            MapPackagesToDto(packages, mappedPackages);
            return mappedPackages;
        }

        private void MapPackagesToDto(IEnumerable<Model.Appv.Package> packages, ICollection<Package> mappedPackages)
        {
            foreach (var package in packages)
            {
                var entitlements = package.Entitlements.Select(x => x.SidString);
                //var entitlementsAsString = entitlements.Select(x => x.SidString);
                var dbPackage =
                    _packageRepository.Query()
                        .FirstOrDefault(x => entitlements.FirstOrDefault(ent => ent == x.PackageSid) != null);
                if (dbPackage == null) continue;

                //var firstOrDefault = _packageRepository.Query().FirstOrDefault(x => x.PackageSid == dbPackage.PackageSid);
                Category category;
                if (dbPackage.Category.Id==0)
                {
                    category = dbPackage.Category;
                }
                else
                {
                    category = new Category {Id = 0, Name = "Unbekannt"};
                }

                mappedPackages.Add(Mapper.PackageToDto(package, dbPackage.Icon,
                    new Dtos.Category {Id = category.Id, Name = category.Name},
                    _appVRepository.GetAppsByPackageId(package.PackageGuid)));
            }
        }

        public IEnumerable<Application> GetApplicationsByPackageId(Guid packageGuid)
        {
           var allApps= _appVRepository.GetAppsByPackageId(packageGuid);
            var result = new List<Application>();
            allApps.ForEach(x=>result.Add(new Application{Id=x.Id,Name = x.Name}));
            return result;
        }

        public IEnumerable<Package> GetUserPackages(SecurityIdentifier userSid)
        {
            var packages = _appVRepository.GetUserPackages(userSid.ToString());
            var mappedPackages = new List<Package>();
            MapPackagesToDto(packages, mappedPackages);
            return mappedPackages;
        }
    }

}
