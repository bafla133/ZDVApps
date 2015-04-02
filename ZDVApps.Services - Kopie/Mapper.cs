using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using ZDVApps.Dtos;
using ZDVApps.Model;
using ZDVApps.Model.AD;
using ZDVApps.Model.DB;
using ZDVApps.Model.DTO;
using Application = ZDVApps.Model.Appv.Application;
using Category = ZDVApps.Dtos.Category;

namespace ZDVApps.Services
{
    public class Mapper
    {
        //static public List<Appv5Package> AdPackagesToAppv5Packages(List<CustomGroupPrincipal> adPackages,bool approved=true)
        //{
        //    var result = new List<Appv5Package>();
        //    adPackages.ForEach(x=>result.Add(new Appv5Package
        //    {
        //        Description = x.Description,
        //        PackageSid = Convert.ToString(x.Sid),
        //        Icon = x.IconPath,
        //        NeedLicense = approved,
        //        Type = PackageTypes.Normal,
        //        LicensesNumber = 0,
        //        Name = x.Name,
        //    }));
        //    return result;
        //}

        //public static List<Group> ADUsersToGroups(List<AppvUser> users)
        //{
        //    var result = new List<Group>();
        //    foreach (var user in users)
        //    {
        //        result.Add(new Group
        //            {
        //                Name = user.Name,
        //                Sid = user.Sid
        //            });
        //    }
        //    return result;
        //}

    

        public static Package PackageToDto(Model.Appv.Package package, string getIcon, Category category, IEnumerable<Application> applications)
        {
            byte[] iconBytes = {};
            if (!String.IsNullOrEmpty(getIcon))
            {
                using(var client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                   iconBytes= client.DownloadData(getIcon);

                }
            }
            var mappedApplications = new List<Dtos.Application>();
            applications.ForEach(x=>mappedApplications.Add(new Dtos.Application
            {
                Name = x.Name,
                Id = x.Id
            }));
            return new Package
            {

                Name = package.Name,
                Guid = package.PackageGuid,
                Icon = iconBytes ?? new byte[0],
                Category = category,
                Applications = mappedApplications
            };
        }
    }
}
