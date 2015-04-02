using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Contracts.WebServices;
using ZDVApps.Dtos;

namespace ZDVApps.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdminWebService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AdminWebService.svc or AdminWebService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class AdminWebService : IAdminWebService
    {
        private readonly IAdminService _adminService;

        public AdminWebService(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IEnumerable<Package> GetAllPackages()
        {
            return _adminService.GetAllPackages();
        }

        public AdUser GetAdUser(string sid)
        {
            return _adminService.GetAdUser(sid);
        }

        public void DepriveUser(string userSid,string groupSid)
        {
            _adminService.DepriveUser(userSid, groupSid);
        }
    }
}
