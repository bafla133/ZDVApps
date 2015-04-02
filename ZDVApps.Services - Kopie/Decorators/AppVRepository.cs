using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Xml;
using System.Xml.Serialization;
using ZDVApps.Common.Contracts;
using ZDVApps.Common.Decorators;
using ZDVApps.Model.Appv;
using ZDVApps.Services.AD;

namespace ZDVApps.Services.Decorators
{
    public class AppVRepository : IAppVRepository, IDisposable
    {
        private bool _disposed;
        private readonly List<Package> packagesList;
        private readonly IActiveDirectoryRepository _activeDirectoryRepository;

        public AppVRepository( IActiveDirectoryRepository activeDirectoryRepository)
        {
            var source = Properties.Settings.Default.API_Appv;
            _activeDirectoryRepository = activeDirectoryRepository;

#if !DEBUG
            var xml_rdr = new XmlTextReader(@"C:\appv5_packages.xml");
#endif

#if DEBUG
             var  xml_rdr = new XmlTextReader(@"C:\appv5_packages.xml");
#endif
            //var strm= xml_rdr.st
            //var reader = XmlReader.Create(strm_rdr);

                var ser = new XmlSerializer(typeof(ArrayOfPackageVersion));

                var packages = (ArrayOfPackageVersion)ser.Deserialize(xml_rdr);

                packagesList = new List<Package>();
                packages.Packages.ForEach(x => packagesList.Add(new Package
                {
                   Applications = x.Applications,
                   Entitlements= x.Entitlements,
                   Enabled = x.Enabled,
                   ConnectionGroups = x.ConnectionGroups,
                   Description = x.Description,
                   Id = x.Id,
                   Name = x.Name,
                   PackageGuid = x.PackageGuid,
                   PackageUrl = x.PackageUrl,
                   TimeChanged = x.TimeChanged,
                   Version = x.Version,
                   VersionGuid = x.VersionGuid
                }));
        }


        public IEnumerable<Package> GetAllPackages()
        {
            return packagesList;
        }

        public IEnumerable<Package> GetUserPackages(string sid)
        {
            var packagesOfUser = new List<Package>();

            var user = _activeDirectoryRepository.GetAdUser(new SecurityIdentifier(sid));

            foreach (var package in packagesList)
            {
                foreach (var entitlement in package.Entitlements)
                {
                    //var group = _activeDirectoryRepository.GetAdGroup(entitlement.SidString);

                    if (user.Groups.ToList().Exists(x => x.ToString() == entitlement.SidString))
                    {
                        packagesOfUser.Add(new Package()
                        {
                            Id = package.Id,
                            Name = package.Name
                        });
                        break;
                    }
                }
            }

            return packagesOfUser;
        }

        public IEnumerable<Package> GetUserUnassignedPackages(string userSid)
        {
            //var user = _activeDirectoryRepository.GetAdUser(new SecurityIdentifier(userSid));
                //var user = _activeDirectoryRepository.AdUsersCache.GetAsGroupMemberDTO(sid);

            //var availablePackagesOfUser = packagesList.Where(package => package.Entitlements.Any(entitlement => 
              //  user.Groups.Exists(x => x.ToString() == entitlement.SidString))).ToList();

            throw new NotImplementedException();
            
        }

        /// <summary>
        /// This method should not be called
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public Package GetPackage(int packageId)
        {

            throw new NotImplementedException();
        }

        public bool IsAssigned(int packageId, string userSid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Package> GetDependenciesOfPackage(int packageId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Package> FilterPackagesByName(string filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Package> FilterUserPackagesByName(string filter, string userSid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Package> FilterUnassignedPackages(string filter, string userSid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAllEntitlements()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dtos.Application> GetAppsByEntitlement(string sid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Application> GetAppsByPackageId(Guid packageGuid)
        {
            var result = packagesList.FirstOrDefault(x => x.PackageGuid == packageGuid);
            if(result==null)
                return new List<Application>();
            return result.Applications;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
