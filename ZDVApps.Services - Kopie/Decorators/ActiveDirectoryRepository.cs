using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Decorators;
using ZDVApps.Dtos;
using ZDVApps.Model.DTO;
using ZDVApps.Services.AD;
using ZDVApps.Services.Properties;
using Package = ZDVApps.Model.Appv.Package;

namespace ZDVApps.Services.Decorators
{
    public class ActiveDirectoryRepository :IActiveDirectoryRepository
    {
        private bool _disposed;
        //private SiteRepository _repository = new SiteRepository();

        private IAppVRepository _appvRepository;

        public ActiveDirectoryRepository(IAppVRepository appvRepository)
        {
            _appvRepository = appvRepository;
        }

        public AppvUser Get(string sid)
        {
            throw new NotImplementedException();
            //var userAsADUser = GetADUser(sid);
            //return MapObjects.MapADUserToADUserDTO(userAsADUser);
        }

        //public Group GetAsGroupMemberDTO(string sid)
        //{
        //    throw new NotImplementedException();
        //    //var userAsADUser = Get(sid);
        //    //return MapObjects.MapADUserToGroupMemberDTO(userAsADUser);
        //}

        public bool AssignGroup(string userSid, string groupSid)
        {
                var usersContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
                var groupContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryGlobalContainer);
                var user = UserPrincipal.FindByIdentity(usersContext, IdentityType.Sid, userSid);
                var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, groupSid);
                if (group == null || user == null)
                    return false;
                group.Members.Add(user);
                group.Save();
                return true;
        }

        //public bool RequestAddingPendingPackage(string userSid, string groupSid, string responsibleSid)
        //{
        //    // Check if the responsible person allowed to decide
        //    var userContext = Contexts.CreateContext("Domain Of Responsible Users");
        //    var responsibleUser = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, responsibleSid);
        //    if (responsibleUser == null) return false;
        //    //send Email to the responsible user so that he decide 
        //    Email.SendMail("mhabibal@uni-mainz.de", "subject", "body", "mhabibal@uni-mainz.de", "testos1");
        //    return true;
        //}

        //public bool DepriveGroup(string userSid, string groupSid, string responsibleSid)
        //{
        //    throw new NotImplementedException();
        //}

        public bool DepriveGroup(string userSid, string groupSid)
        {
            var groupContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryGlobalContainer);
            var user = UserPrincipal.FindByIdentity(groupContext, IdentityType.Sid, userSid);
            var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, groupSid);

            if (group == null)
                throw new Exception("Group not found");

            if (user == null)
                throw new Exception("AppvUser hasnt been found");

            group.Members.Remove(user);
            group.Save();
            return true;
        }

        public bool CheckUserIsPriviliged(string userSid)
        {
            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerEmployee);
            var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, userSid);
            return user != null;
        }

        private AppvUser GetADUser(string sid)
        {
            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
            var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, sid);
            return new AppvUser
                       {
                           Email = user.EmailAddress,
                           Name = user.Name,
                           IsPriviliged = IsPriviliged(sid),
                           Sid = sid
                       };
        }

        //public string GetSid(string userName)
        //{
        //    throw new NotImplementedException();
        //}

        //public string GetMail(string sid)
        //{
        //    var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
        //    var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, sid);
        //    return user.EmailAddress;
        //}

        public AdUser GetAdUser(SecurityIdentifier sid)
        {
            if(sid==null) return null;
            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
            var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, sid.ToString());
            if (user == null) return null;
            return new AdUser { Name = user.Name, SecurityIdentifier = user.Sid.ToString(), Mail = user.EmailAddress, Groups = user.GetGroups().Select(x => x.Sid.ToString()) };

        }

        public AdUser GetAdUserByName(string name)
        {
            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
            var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Name, name);
            if (user == null) return null;
            return new AdUser { Name = user.Name, SecurityIdentifier = user.Sid.ToString(), Mail = user.EmailAddress, Groups = user.GetGroups().Select(x => x.Sid.ToString()) };
        }

        public AdGroup GetAdGroup(string sidString)
        {

            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerAppVGroups);
            var group = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, sidString);
            if (group == null) return null;
            return new AdGroup { Name = group.Name, Sid = group.Sid};
        }

        public bool IsPriviliged(string userName)
        {
            var uniWorkerContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerEmployee);
            var user = UserPrincipal.FindByIdentity(uniWorkerContext, IdentityType.Name, userName);
            return user != null;
        }

        //public bool IsMember(string userName, Principal groupName)
        //{
        //    var userContext = Contexts.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
        //    var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Name, userName);
        //    var userGroups = user.GetAuthorizationGroups().ToList();
        //    return userGroups.Contains(groupName);
        //}

        //public string GetEmail(string userName)
        //{
        //    var userContext = Contexts.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
        //    var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Name, userName);
        //    return user == null ? "" : user.EmailAddress;
        //}

        public IEnumerable<Package> GetAdministrativeAssignedPackages(string sid){
            
        
            var userContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
            var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, sid);
            var groupsAsList = user.GetGroups().ToList();
            var authorizedGroupsAsList = user.GetAuthorizationGroups().ToList();
            var sharedGroups = new List<Principal>();
            foreach (var group in groupsAsList)
            {
                if (authorizedGroupsAsList.FirstOrDefault(x => x.Sid.Equals(group.Sid)) != null)
                {
                    sharedGroups.Add(group);
                }
            }
            foreach (var group in sharedGroups)
            {
                authorizedGroupsAsList.Remove(group);
            }

            var listOfAssignedPackages = new List<Package>();

                var repoGroups = _appvRepository.GetUserPackages(sid);
                foreach (var package in repoGroups)
                {
                    foreach (var entitlement in package.Entitlements)
                    {
                        if (authorizedGroupsAsList.Contains(GetGroup(entitlement.SidString)))
                        {
                            listOfAssignedPackages.Add(package);
                            break;
                        }
                    }
                
            }
            
            return listOfAssignedPackages;
        }



        public GroupPrincipal GetGroup(string sid)
        {

            var groupContext = ContextBuilder.CreateContext(Settings.Default.ActiveDirectoryGlobalContainer);
            var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, sid);
            return group;
        }

        //public List<Principal> GetNotAvailableGroups(string userName)
        //{
        //    var userContext = Contexts.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
        //    var allAppVPackages = GetAllAppVPackagesAsPrincipal();
        //    var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Name, userName);
        //    var userPackages = user.GetAuthorizationGroups().ToList();
        //    var userNotAvailableAppVPackages = allAppVPackages.FindAll(x => !userPackages.Contains(x));
        //    return userNotAvailableAppVPackages.ToList();
        //}

        //private List<Principal> GetAllAppVPackagesAsPrincipal()
        //{
        //    var contextApproval =
        //        new GroupPrincipal(Contexts.CreateContext(Settings.Default.ActiveDirectoryContainerApproval));
        //    var searcherApproval = new PrincipalSearcher(contextApproval);
        //    var appsGroupsAppProval = searcherApproval.FindAll().ToList();
        //    var contextSelfService =
        //        new GroupPrincipal(Contexts.CreateContext(Settings.Default.ActiveDirectoryContainerSelfService));
        //    var searcherSelfService = new PrincipalSearcher(contextSelfService);
        //    var selfServicePackages = searcherSelfService.FindAll().ToList();
        //    appsGroupsAppProval.AddRange(selfServicePackages);
        //    return appsGroupsAppProval;
        //}

        //public List<Principal> GetAvailablePackages(string userName)
        //{
        //    var context = Contexts.CreateContext(Settings.Default.ActiveDirectoryContainerUsers);
        //    var user = UserPrincipal.FindByIdentity(context, IdentityType.Name, userName);
        //    var userGroups = user.GetAuthorizationGroups().ToList();
        //    var allGroups = GetAllAppVPackagesAsPrincipal();
        //    var userAppVGroups = userGroups.Intersect(allGroups);
        //    return userAppVGroups.ToList();
        //}
    }
    }

