using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using ZDVApps.Common.Contracts;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Contracts.WebServices;
using ZDVApps.Model.Appv;
using Application = ZDVApps.Dtos.Application;

namespace ZDVApps.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ZDVApps" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ZDVApps.svc or ZDVApps.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [DataContract]
    public class ZDVApps : IZDVApps
    {
        private IAppVService _appvService;


        public ZDVApps(IAppVService appVService)
        {
            _appvService = appVService;
        }
        public IEnumerable<Dtos.Package> GetAllPackages()
        {
            var result=_appvService.GetAllPackages();
            return result;
        }

        public IEnumerable<Application> GetApplicationsByPackageId(Guid packageGuid)
        {
            var result = _appvService.GetApplicationsByPackageId(packageGuid);
            return result;
        }
        public IEnumerable<Dtos.Package> UserPackages(string userSid)
        {
            var result = _appvService.GetUserPackages(new SecurityIdentifier(userSid));
            return result;
        }
    }
}
