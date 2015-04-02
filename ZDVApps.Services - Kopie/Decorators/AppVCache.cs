using System;
using System.Collections.Generic;
using System.Linq;
using ZDVApps.Common.Contracts;
using ZDVApps.Common.Decorators;
using ZDVApps.Dtos;
using ZDVApps.Model.DTO;
using ZDVApps.Services.Properties;
using Application = ZDVApps.Model.Appv.Application;
using Package = ZDVApps.Model.Appv.Package;

namespace ZDVApps.Services.Decorators
{
    public class AppVCache : AppVRepositoryDecorator
    {
        private IEnumerable<Package> _packagesList = new List<Package>();

        public AppVCache(IAppVRepository repository) : base(repository)
        {
        }

        public override IEnumerable<Package> GetAllPackages()
        {
            if (_packagesList.ToList().Count == 0)
            {
                using (var cacheProvider = new CacheProvider())
                {
                    var cachedPackages = cacheProvider.Get("AllPackages", "Appv5Service") as List<Package>;
                    //var cachedPackages = cacheProvider.Get("AllPackages", Settings.Default.AppvPackages_AllPackagesExtended) as List<Package>;
                    if (cachedPackages == null || cachedPackages.Count == 0)
                    {
                        _packagesList = Repository.GetAllPackages();
                        //cacheProvider.AddToRegion("AllPackagesExtended", _packagesList, new TimeSpan(0, 4, 0, 0), Settings.Default.AppvPackages_AllPackagesExtended);
                        cacheProvider.AddToRegion("AllPackages", _packagesList, new TimeSpan(0, 4, 0, 0), "Appv5Service");
                    }
                    else
                    {
                        _packagesList = cachedPackages;
                    }
                }

            }
            return _packagesList;
        }

        public override IEnumerable<Package> GetUserPackages(string userSid)
        {
            IEnumerable<Package> packagesOfUser;
            GetAllPackages();
            using (var cacheProvider = new CacheProvider())
            {
                var userPackages = cacheProvider.Get(userSid, Settings.Default.UsersPackagesRegion) as IEnumerable<Package>;

                if (userPackages != null)
                    return userPackages;
                
                packagesOfUser = Repository.GetUserPackages(userSid);

                 cacheProvider.AddToRegion(userSid, packagesOfUser, Settings.Default.UsersPackagesRegion);
            }

            return packagesOfUser;
        }

        public override IEnumerable<Package> GetUserUnassignedPackages(string userSid)
        {
            var userPackages = GetUserPackages(userSid);

            var unassignedPackaes = new List<Package>(_packagesList);
            foreach (var package in userPackages)
            {
                var packageMatch = unassignedPackaes.FirstOrDefault(x => x.Id == package.Id);
                unassignedPackaes.Remove(packageMatch);
            }
            return unassignedPackaes;
            
        }

        public override Package GetPackage(int packageId)
        {
            if (_packagesList.ToList().Count != 0) return _packagesList.FirstOrDefault(x => x.Id == packageId);
            using (var cacheProvider = new CacheProvider())
            {
                var cachedPackages = cacheProvider.Get("AllPackages", "Appv5Service") as List<Package>;
                //var cachedPackages = cacheProvider.Get("AllPackages", Settings.Default.AppvPackages_AllPackagesExtended) as List<Package>;
                if (cachedPackages != null && cachedPackages.Count != 0)
                    _packagesList = cachedPackages;

                _packagesList = Repository.GetAllPackages();
                //cacheProvider.AddToRegion("AllPackagesExtended", _packagesList, new TimeSpan(0, 4, 0, 0), Settings.Default.AppvPackages_AllPackagesExtended);
                cacheProvider.AddToRegion("AllPackages", _packagesList, new TimeSpan(0, 4, 0, 0), "Appv5Service");
                
            }
            return _packagesList.FirstOrDefault(x => x.Id == packageId);
        }

        public override bool IsAssigned(int packageId, string userSid)
        {
            var availablePackages = GetUserPackages(userSid);
            return availablePackages.Any(package => package.Id == packageId);
        }

        public override IEnumerable<Package> GetDependenciesOfPackage(int packageId)
        {
            var entitlements = GetPackage(packageId).Entitlements;
            //foreach (var package in GetAllPackages())
            //{
            //    if (package.Entitlements.Select(entitlement => entitlements.FirstOrDefault(x => x.SidString == entitlement.SidString)).
            //        Any(entitlementMatch => entitlementMatch != null))
            //    {
            //        dependedPackages.Add(new Package
            //        {
            //            Id = package.Id,
            //            Name = package.Name
            //        });
            //    }
            //}
            return (from package in GetAllPackages()
                where package.Entitlements.Select(entitlement => entitlements.FirstOrDefault(x => x.SidString == entitlement.SidString)).Any(entitlementMatch => entitlementMatch != null)
                select new Package
                {
                    Id = package.Id, Name = package.Name
                }).ToList();
        }

        public override IEnumerable<Package> FilterPackagesByName(string filter)
        {
            filter = filter.ToLowerInvariant();
            var allPackages = GetAllPackages();
            return Filter(filter, allPackages);
                //allPackages.Where(
                //    x =>
                //        x.Name.ToLowerInvariant().Contains(filter) || x.Description.ToLowerInvariant().Contains(filter) ||
                //        x.Applications.Select(app => app.Name).Any(name => name.ToLowerInvariant().Contains(filter)));
        }

        public override IEnumerable<Package> FilterUserPackagesByName(string filter, string userSid)
        {
            filter = filter.ToLowerInvariant();
            var userPackages = GetUserPackages(userSid);
            return Filter(filter, userPackages);
                //userPackages.Where(
                //    x =>
                //        x.Name.ToLowerInvariant().Contains(filter) || x.Description.ToLowerInvariant().Contains(filter) ||
                //        x.Applications.Select(app => app.Name).Any(name => name.ToLowerInvariant().Contains(filter)));

        }

        public override IEnumerable<Package> FilterUnassignedPackages(string filter, string userSid)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Package> FilterNotAvailablePackagesByUserName(string filter, string userSid)
        {
            filter = filter.ToLowerInvariant();
            var userPackages = GetUserUnassignedPackages(userSid);
            return
                Filter(filter, userPackages);
        }

        private static IEnumerable<Package> Filter(string filter, IEnumerable<Package> userPackages)
        {
            return userPackages.Where(
                x =>
                    x.Name.ToLowerInvariant().Contains(filter) || x.Description.ToLowerInvariant().Contains(filter) ||
                    x.Applications.Select(app => app.Name).Any(name => name.ToLowerInvariant().Contains(filter)));
        }

        public override IEnumerable<string> GetAllEntitlements()
        {
            var allPackages = GetAllPackages();
            var listOfEntitlements = new HashSet<string>();
            foreach (var package in allPackages)
            {
                package.Entitlements.ToList().ForEach(x => listOfEntitlements.Add(x.SidString));
            }
            return listOfEntitlements.ToList();
        }

        public override IEnumerable<Dtos.Application> GetAppsByEntitlement(string sid)
        {
            var appsList = new List<Dtos.Application>();
            var allPackages = GetAllPackages();

            foreach (var package in allPackages)
            {
                if (ContainsEntitlement(package, sid))
                {
                    foreach (var app in package.Applications)
                    {
                        appsList.Add(new Dtos.Application
                        {
                            Id = Convert.ToInt16(app.AppId),
                            Name = app.Name
                        });
                    }
                }

            }
            return appsList;
        }

        public override IEnumerable<Application> GetAppsByPackageId(Guid packageGuid)
        {
            if (_packagesList.ToList().Count == 0)
            {
                using (var cacheProvider = new CacheProvider())
                {
                    var cachedPackages = cacheProvider.Get("AllPackages", "Appv5Service") as List<Package>;
                    if (cachedPackages == null || cachedPackages.Count == 0)
                    {
                        _packagesList = Repository.GetAllPackages();

                        cacheProvider.AddToRegion("AllPackages", _packagesList, new TimeSpan(0, 4, 0, 0), "Appv5Service");
                    }
                    else
                    {
                        _packagesList = cachedPackages;
                    }
                }

            }
            return _packagesList.First(x => x.PackageGuid == packageGuid).Applications;
        }
        private bool ContainsEntitlement(Package package, string sid)
        {
            foreach (var entitlement in package.Entitlements)
            {
                if (entitlement.ToString().Equals(sid))
                    return true;
            }
            return false;
        }
    }
}
