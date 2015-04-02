using System;
using System.Collections.Generic;
using System.Security.Principal;
using ZDVApps.Common.Decorators;
using ZDVApps.Dtos;
using ZDVApps.Model.DTO;
using ZDVApps.Services.Properties;
using Package = ZDVApps.Model.Appv.Package;

namespace ZDVApps.Services.Decorators
{
    public class ActiveDirectoryRepositoryCache : ActiveDirectoryRepositoryDecorator
    {

        public ActiveDirectoryRepositoryCache(IActiveDirectoryRepository activeDirectoryRepository)
            : base(activeDirectoryRepository)
        {
        }

        //public override AppvUser Get(string sid)
        //{
        //    AppvUser repositoryAppvUser;
        //    using (var cacheProvider = new CacheProvider())
        //    {
        //        var cachedUser = cacheProvider.Get(sid, "ADUsers") as AppvUser;
        //        if (cachedUser != null)
        //            return cachedUser;
        //         repositoryAppvUser = activeDirectoryRepository.GetAdUser(new SecurityIdentifier(sid));
        //        cacheProvider.AddToRegion(repositoryAppvUser.Sid, repositoryAppvUser, "");

        //    }
        //    return repositoryAppvUser; 
        //}

        //public override Group GetAsGroupMemberDTO(string sid)
        //{
        //    Group member;
        //    using (var cacheProvider = new CacheProvider())
        //    {

        //        var cachedUser = cacheProvider.Get(sid, "ADUsers") as AppvUser;
        //        if (cachedUser != null)
        //            return new Group{Name = cachedUser.Name,Sid = cachedUser.Sid};
        //        var repositoryUser = activeDirectoryRepository.Get(sid);
        //        cacheProvider.AddToRegion(repositoryUser.Sid, repositoryUser, "ADUsers");
        //        member = new Group { Name = repositoryUser.Name, Sid = repositoryUser.Sid };
        //    }
        //    return member;

        //}

        public override AppvUser Get(string userName)
        {
            throw new NotImplementedException();
        }

        //public override Group GetAsGroupMemberDTO(string sid)
        //{
        //    throw new NotImplementedException();
        //}

        public override bool AssignGroup(string userSid, string groupSid)
        {
            using (var cacheProvider = new CacheProvider())
            {
                ActiveDirectoryRepository.AssignGroup(userSid, groupSid);
                cacheProvider.Remove(userSid, "ADUsers");
                cacheProvider.Remove(groupSid, "ADGroups");
            }
            return true;
        }



        public override bool CheckUserIsPriviliged(string userSid)
        {
            bool isPrviliged;
            using (var cacheProvider = new CacheProvider())
            {
                isPrviliged = Convert.ToBoolean(cacheProvider.Get(userSid, "IsPriviliged"));
                if (isPrviliged) return true;

                isPrviliged = ActiveDirectoryRepository.CheckUserIsPriviliged(userSid);
                cacheProvider.AddToRegion(userSid, isPrviliged, "IsPriviliged");
                return isPrviliged;
            }

        }

        public override bool DepriveGroup(string userSid, string groupSid)
        {
            using (var cacheProvider = new CacheProvider())
            {
                ActiveDirectoryRepository.DepriveGroup(userSid, groupSid);
                cacheProvider.Remove(userSid, "ADUsers");
                cacheProvider.Remove(groupSid, "ADGroups");
            }

            return true;
        }

        public override IEnumerable<Package> GetAdministrativeAssignedPackages(string sid)
        {
            using (var cacheProvider = new CacheProvider())
            {
                var cachedGroups = cacheProvider.Get(sid, "AdministrativePackages");
                if (cachedGroups != null)
                    return (List<Package>) cachedGroups;
                cachedGroups = ActiveDirectoryRepository.GetAdministrativeAssignedPackages(sid);
                cacheProvider.AddToRegion(sid, cachedGroups, "AdministrativePackages");
                return (List<Package>) cachedGroups;
            }

        }

        //public override string GetSid(string userName)
        //{
        //    string cachedUserSid;
        //    using (var cacheProvider = new CacheProvider())
        //    {
        //        cachedUserSid = cacheProvider.Get(userName, "ADUserSids") as string;
        //        if (cachedUserSid != null)
        //            return cachedUserSid;
        //        cachedUserSid = ActiveDirectoryRepository.GetSid(userName);
        //        cacheProvider.AddToRegion(userName, cachedUserSid, "ADUserSids");
        //    }
        //    return cachedUserSid;
        //}


        //public override string GetMail(string sid)
        //{
        //    return ActiveDirectoryRepository.GetMail(sid);
        //}

        public override AdUser GetAdUser(SecurityIdentifier sid)
        {
            if (sid == null) return null;
            AdUser user;
            using (var cacheProvider = new CacheProvider())
            {
                user = cacheProvider.Get(sid.ToString(), "ADUsers") as AdUser;
                if (user != null)
                    return user;

                user = ActiveDirectoryRepository.GetAdUser(sid);
                cacheProvider.AddToRegion(sid.ToString(), user, "ADUsers");
                return user;
            }
        }

        public override AdUser GetAdUserByName(string name)
        {
            AdUser user;
            using (var cacheProvider = new CacheProvider())
            {
                user = cacheProvider.Get(name, "ADUsers_Name") as AdUser;
                if (user != null)
                    return user;

                user = ActiveDirectoryRepository.GetAdUserByName(name);
                cacheProvider.AddToRegion(name, user, "ADUsers_Name");
                return user;
            }
        }

        public override AdGroup GetAdGroup(string sidString)
        {
            AdGroup adGroup;
            using (var cacheProvider = new CacheProvider())
            {
                adGroup = cacheProvider.Get(sidString, "ADGroups") as AdGroup;
                if (adGroup != null)
                    return adGroup;

                adGroup = ActiveDirectoryRepository.GetAdGroup(sidString);
                cacheProvider.AddToRegion(sidString, adGroup, "ADGroups");

            }
            return adGroup;
        }
    }
}
