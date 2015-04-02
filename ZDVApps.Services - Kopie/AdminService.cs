using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Decorators;
using ZDVApps.Dtos;

namespace ZDVApps.Services
{
    public class AdminService : IAdminService
    {
        private IAppVService _appVService;
        private readonly IActiveDirectoryRepository _activeDirectoryRepository;

        public AdminService(IActiveDirectoryRepository activeDirectoryRepository, IAppVService appVService)
        {
            _activeDirectoryRepository = activeDirectoryRepository;
            _appVService = appVService;
        }

        public IEnumerable<Package> GetAllPackages()
        {
            return _appVService.GetAllPackages();
        }

        public AdUser GetAdUser(string sid)
        {
            return _activeDirectoryRepository.GetAdUser(new SecurityIdentifier(sid));
        }

        public void DepriveUser(string userSid,string groupSid)
        {
            _activeDirectoryRepository.DepriveGroup(userSid, groupSid);
        }
    }
}
