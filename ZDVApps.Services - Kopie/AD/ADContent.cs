using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using ZDVApps.Common.Contracts;
using ZDVApps.Model.AD;
using ZDVApps.Services.Properties;

namespace ZDVApps.Services.AD
{

    public static class AdContent 

    {
        #region Contexts

        public static PrincipalContext CurrentDomainContextApproval
        {
            get
            {
                
                var currentDomain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
                var context = new System.DirectoryServices.AccountManagement.PrincipalContext
                    (ContextType.Domain, currentDomain, Settings.Default.ActiveDirectoryContainerApproval);
                return context;
            }
        }
        //private static System.DirectoryServices.AccountManagement.PrincipalContext CurrentDomainContextAppVPacakges
        //{
        //    get
        //    {
        //        var currentDomain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
        //        var context = new System.DirectoryServices.AccountManagement.PrincipalContext
        //            (ContextType.Domain, currentDomain, Settings.Default.ActiveDirectoryContainerAppVPackages);
        //        return context;
        //    }
        //}
        private static System.DirectoryServices.AccountManagement.PrincipalContext CurrentDomainContextAppvGroups
        {
            get
            {
                var currentDomain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
                var context = new System.DirectoryServices.AccountManagement.PrincipalContext
                    (ContextType.Domain, currentDomain, Settings.Default.ActiveDirectoryContainerAppVGroups);
                return context;
            }
        }

        private static System.DirectoryServices.AccountManagement.PrincipalContext CurrentDomainContextSelfService
        {
            get
            {
                var currentDomain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
                var context = new System.DirectoryServices.AccountManagement.PrincipalContext
                    (ContextType.Domain, currentDomain, Settings.Default.ActiveDirectoryContainerSelfService);
                return context;
            }
        }


        private static PrincipalContext CurrentDomainContextGlobal
        {
            get
            {
                var currentDomain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
                var context = new PrincipalContext
                    (ContextType.Domain, currentDomain, Settings.Default.ActiveDirectoryGlobalContainer);
                return context;
            }
        }
        #endregion
        #region GetMethods
        //public static AdPackage GetPackage(string groupName)
        //{
        //    var contextApproval = new CustomGroupPrincipal(CurrentDomainContextApproval){Name =groupName};

        //    var searcherApproval = new PrincipalSearcher(contextApproval);
        //    var appsGroupsAppProval = searcherApproval.FindAll().Cast<CustomGroupPrincipal>().AsEnumerable();
        //    var app = appsGroupsAppProval.FirstOrDefault();
        //    if (app==null)
        //    {
        //        var contextSelfService = new CustomGroupPrincipal(CurrentDomainContextSelfService);
        //        var searcherSelfService = new PrincipalSearcher(contextSelfService);
        //        var appsGroupsSelfService = searcherSelfService.FindAll().Cast<CustomGroupPrincipal>().ToList();
        //        app = appsGroupsSelfService.FirstOrDefault();
        //        return new AdPackage
        //                   {
        //                       Name = app.Name,
        //                       NeedLicense = true,
        //                       IconPath = app.IconPath
        //                   };
        //    }
        //    return new AdPackage
        //    {
        //        Name = app.Name,
        //        NeedLicense = true,
        //        IconPath = app.IconPath
        //    };
        //}
    
        public static GroupPrincipal GetGroupBySid(string sid)
        {

            using (var context = CurrentDomainContextGlobal)
            {
                var appVPacakge = GroupPrincipal.FindByIdentity(context, IdentityType.Sid, sid);
                return appVPacakge;
            }
        }

        public static Guid GetGuidByName(string groupName)
        {
            using (var context = CurrentDomainContextAppvGroups)
            {
                var group = GroupPrincipal.FindByIdentity(context, IdentityType.Name, groupName);
                return group==null? new Guid() : group.Guid.GetValueOrDefault();
            }
        }

        //public static Guid GetGuidOfUser(string userName)
        //{
        //    using (var context = CurrentDomainContextUser)
        //    {
        //        var user = UserPrincipal.FindByIdentity(context, IdentityType.Guid, userName);
        //        return user.Guid.GetValueOrDefault();
        //    }
        //}
        //public static List<ADPackage> GetAllAppVPackagesAsADPackagesList()
        //{
        //    var contextApproval = new CustomGroupPrincipal(CurrentDomainContextApproval);
        //    var searcherApproval = new PrincipalSearcher(contextApproval);
        //    var appsGroupsAppProval = searcherApproval.FindAll().Cast<CustomGroupPrincipal>().AsEnumerable();

        //    var packages = new List<ADPackage>();
        //    Mapper.CreateMap<CustomGroupPrincipal, ADPackage>();
        //    foreach (var groupPrincipal in appsGroupsAppProval)
        //    {
        //        var mappedGroup = Mapper.Map<CustomGroupPrincipal, ADPackage>(groupPrincipal);

        //        string[] splittedList;
        //        if (groupPrincipal.Description != null)
        //        {
        //            splittedList = groupPrincipal.Description.Split(';');
        //            mappedGroup.Categories = splittedList.ElementAt(0);
        //            mappedGroup.Name = splittedList.ElementAt(1);
        //            mappedGroup.Description = splittedList.ElementAt(2);
        //        }
        //        else
        //        {
        //            mappedGroup.Categories = "CategoryNotFound";
        //            mappedGroup.Name = groupPrincipal.Name;
        //            mappedGroup.Description= groupPrincipal.Description;
        //        }
        //        packages.Add(mappedGroup);
        //    }

        //    var contextSelfService = new CustomGroupPrincipal(CurrentDomainContextSelfService);
        //    var searcherSelfService = new PrincipalSearcher(contextSelfService);
        //    var appsGroupsSelfService = searcherSelfService.FindAll().Cast<CustomGroupPrincipal>().ToList();

        //    foreach (var groupPrincipal in appsGroupsSelfService)
        //    {
        //        var mappedGroup = Mapper.Map<CustomGroupPrincipal, ADPackage>(groupPrincipal);
        //        string[] splittedList;
        //        if (groupPrincipal.Description != null)
        //        {
        //            splittedList = groupPrincipal.Description.Split(';');
        //            mappedGroup.Categories = splittedList.ElementAt(0);
        //            mappedGroup.Name = splittedList.ElementAt(1);
        //            mappedGroup.Description = splittedList.ElementAt(2);
        //        }
        //        else
        //        {
        //            mappedGroup.Categories = "CategoryNotFound";
        //            mappedGroup.Name = groupPrincipal.Name;
        //            mappedGroup.Description = groupPrincipal.Description;
        //        }
        //        mappedGroup.NeedUnlock = true;
        //        packages.Add(mappedGroup);
        //    }
        //    return packages;
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Appv")]
        public static IEnumerable<CustomGroupPrincipal> GetAllAppv5Packages()
        {
            List<CustomGroupPrincipal> appsGroupsAppProval;
            List<CustomGroupPrincipal> selfServicePackages;

            using (var groupPrincipal = new CustomGroupPrincipal(CurrentDomainContextApproval))
            {
                var groupPrincipalSearcher = new PrincipalSearcher(groupPrincipal);
                appsGroupsAppProval = groupPrincipalSearcher.FindAll().Cast<CustomGroupPrincipal>().ToList();
                appsGroupsAppProval.ForEach(x=>x.NeedLicense=true);
                //Category, PackageName and Description are saved under the same attributes, spliten with ;
                foreach (var package in appsGroupsAppProval)
                {
                    if (!string.IsNullOrEmpty(package.Description))
                    {
                        var info = package.Description.Split(';');
                        package.Category = info[0];
                        if (info.Length > 1)
                            package.Name = info[1];
                        if (info.Length > 2)
                            package.Description = info[2];
                    }
                    else
                    {
                        package.Description = "";
                        
                        package.Category = "";
                    }

                }
            }

            using (var groupPrincipal = new CustomGroupPrincipal(CurrentDomainContextSelfService))
            {
                var groupPrincipalSearcher = new PrincipalSearcher(groupPrincipal);
                selfServicePackages = groupPrincipalSearcher.FindAll().Cast<CustomGroupPrincipal>().ToList();
               
            }
            appsGroupsAppProval.AddRange(selfServicePackages);

            foreach (var package in appsGroupsAppProval)
            {
                if (!string.IsNullOrEmpty(package.Description))
                {
                    var info = package.Description.Split(';');
                    package.Category = info[0];
                    if (info.Length > 1)
                        package.Name = info[1];
                    if (info.Length > 2)
                        package.Description = info[2];
                }
                else
                {
                    package.Description = "";

                    package.Category = "";
                }

            }
            return appsGroupsAppProval;

        }
   
        //public static List<Principal> GetUserAppVPackagesAsPrincipal(string userName)
        //{
        //    using (var context = CurrentDomainContextUser)
        //    {
        //        var user = UserPrincipal.FindByIdentity(context, IdentityType.Name, userName);
        //        var userGroups = user.GetAuthorizationGroups().ToList();
        //        var allGroups = GetAllAppVPackagesAsPrincipal();
        //        var userAppVGroups = userGroups.Intersect(allGroups);

        //        return userAppVGroups.ToList();
        //    }
        //}
        //public static List<Principal> GetUserNotAvailableAppVPackages(string userName)
        //{
        //    using (var context = CurrentDomainContextUser)
        //    {
        //        var allAppVPackages = GetAllAppVPackagesAsPrincipal();
        //        var user = UserPrincipal.FindByIdentity(context, IdentityType.Name, userName);
        //        var userPackages=user.GetAuthorizationGroups().ToList();
        //        var userNotAvailableAppVPackages = allAppVPackages.FindAll(x => !userPackages.Contains(x));
        //        return userNotAvailableAppVPackages.ToList();
        //    }
        //}

        public static UserPrincipal GetUser(string userName)
        {
            using (var userContext = CurrentDomainContextGlobal)
            {
                var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Name, userName);
                return user;
            }
        }
        public static UserPrincipal GetUser(SecurityIdentifier userSid)
        {
            using (var userContext = CurrentDomainContextGlobal)
            {
                var user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, userSid.ToString());
                return user;
            }
        }
       

        public static List<Principal> GetApprovalPackages()
        {
            using (CurrentDomainContextApproval)
            {
                using (var contextApproval = new GroupPrincipal(CurrentDomainContextApproval))
                {
                    var searcherApproval = new PrincipalSearcher(contextApproval);
                    var appsGroupsAppProval = searcherApproval.FindAll().ToList();
                    return appsGroupsAppProval;
                }
            }
        }

        public static List<CustomGroupPrincipal> GetSelfServicePackages()
        {
            using (CurrentDomainContextSelfService)
            {
                using (var contextSelfService = new GroupPrincipal(CurrentDomainContextApproval))
                {
                    var searcherSelfService = new PrincipalSearcher(contextSelfService);
                    var appsGroupsAppProval = searcherSelfService.FindAll().Cast<CustomGroupPrincipal>().ToList();
                    return appsGroupsAppProval;
                }
            }
        }

        #endregion

        #region AddMethods

        public static void AddMemberToGroup(UserPrincipal user, string sid)
        {
            using (var groupContext = CurrentDomainContextGlobal)
            {
                var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, sid);
                group.Members.Add(user);
                group.Save();
            }
        }

        #endregion

        #region RemoveMethods

        public static void RemoveMemberFromGroup(UserPrincipal user, string sid)
        {
            using (var groupContext = CurrentDomainContextGlobal)
            {
                var group = GroupPrincipal.FindByIdentity(groupContext, IdentityType.Sid, sid);
                if (@group == null) return;
                @group.Members.Remove(user);
                @group.Save();
            }
        }

        #endregion

        #region CheckMethods

        #endregion

    }
}
