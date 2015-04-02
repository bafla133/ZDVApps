using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Contracts;
using ZDVApps.Common.Contracts.Services;
using ZDVApps.Common.Decorators;
using ZDVApps.Model.Appv;
using ZDVApps.Services.AD;
using ZDVApps.Services.Properties;

namespace ZDVApps.Services
{
    public class UserService : IUserService
    {
        private IAppVRepository _appVRepository;
        //private readonly Lazy<IAppVRepository> _appV;
        public UserService(IAppVRepository appVRepository)
        {
            _appVRepository = appVRepository;
            //_appV = new Lazy<IAppVRepository>(() => new AppVCache(new AppVRepository(Properties.Settings.Default.API_Appv)));
        }
        public IEnumerable<Package> GetAllPackages()
        {
            var results = _appVRepository.GetAllPackages();
            return results;
            //var result=_appV.Value.GetAllPackages();
            //return result;
        }

        public void AssignGroup(string userSid, IEnumerable<string> groups)
        {
            try
            {
                var groupContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryGlobalContainer);
                var user = UserPrincipal.FindByIdentity(groupContext, IdentityType.Sid, userSid);
                foreach (var groupSid in groups)
                {
                    var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, groupSid);
                    if (group != null && user != null)
                    {
                        group.Members.Add(user);
                        group.Save();
                    }

                }

            }
            catch (Exception)
            {
            }
        }

        public void DepriveGroup(string userSid, IEnumerable<string> groups)
        {
            var groupContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryGlobalContainer);
            var user = UserPrincipal.FindByIdentity(groupContext, IdentityType.Sid, userSid);
            foreach (var groupSid in groups)
            {
                var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, groupSid);
                if (group != null && user != null)
                {
                    group.Members.Remove(user);
                    group.Save();
                }
            }
        }

        public IEnumerable<Package> AdministrativeAssignedPackages(string userSid)
        {
            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
            var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, userSid);

            var groups = user.GetGroups().ToList();
            var authorizedGroupsAsList = user.GetAuthorizationGroups().ToList();
            var sharedGroups = new List<Principal>();
            foreach (var group in groups)
            {
                if (authorizedGroupsAsList.FirstOrDefault(x => x.Sid.Equals(group.Sid)) != null)
                    sharedGroups.Add(group);
            }
            foreach (var group in sharedGroups)
                authorizedGroupsAsList.Remove(group);


            var listOfAssignedPackages = new List<Package>();

            var repoGroups = GetPackages(userSid);
            foreach (var package in repoGroups)
            {
                foreach (var entitlement in package.Entitlements)
                {
                    if (authorizedGroupsAsList.Contains(GetAdGroup(entitlement.SidString)))
                    {
                        listOfAssignedPackages.Add(package);
                        break;
                    }
                }
            }
            

            return listOfAssignedPackages;
        }

        public string GetMail(string sid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Package> GetPackages(string usersid)
        {
            throw new NotImplementedException();
        }

        private GroupPrincipal GetAdGroup(string groupSid)
        {
            var groupContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerAppVGroups);
            var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, groupSid);
            return group;
        }

    }

    
}
