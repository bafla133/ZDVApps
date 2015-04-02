using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using ZDVApps.Common;
using ZDVApps.Common.Decorators;
using ZDVApps.DataAccess;
using ZDVApps.Common.Contracts;
using ZDVApps.DataTransfer.Contracts;
using ZDVApps.Model.AD;
using ZDVApps.Model.Appv;
using ZDVApps.Model.DB;
using ZDVApps.Services.AD;
using ZDVApps.Services.Properties;
using PackageTypes = ZDVApps.Model.PackageTypes;
using System.IO.Compression;
namespace ZDVApps.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Appv5Package> _appv5Repository;
        private readonly IRepository<Category> _appv5Category;
        private readonly IRepository<Appv5Application> _applicationRepository;
        private readonly IAppVRepository _appvRepository;

        public SynchronizationService(IUnitOfWork unitOfWork, IRepository<Appv5Package> appv5Repository,IRepository<Category> appv5Category,IRepository<Appv5Application> applicationRepository,
        IAppVRepository appvRepository)
        {
            _unitOfWork = unitOfWork;
            _appv5Repository = appv5Repository;
            _appv5Category = appv5Category;
            _applicationRepository = applicationRepository;
            _appvRepository = appvRepository;
            using (var cacheProvider = new CacheProvider())
            {
                cacheProvider.ResetCache();
            }
            
        }

        private void SynchronizePackages(IEnumerable<CustomGroupPrincipal> appv5Packages)
        {
            var mappedPackages = new List<Appv5Package>();
            foreach (var appvPackage in appv5Packages)
            {

                var mappedPackage = new Appv5Package
                {
                    Description = appvPackage.Description,
                    PackageSid = Convert.ToString(appvPackage.Sid),
                    Icon =  appvPackage.IconPath,
                    NeedLicense = appvPackage.NeedLicense,
                    Type = PackageTypes.Normal,
                    LicensesNumber = 0,
                    Name = appvPackage.Name,
                    Category = _appv5Category.Query().FirstOrDefault(x => x.Name == appvPackage.Category) ?? _appv5Category.Query().FirstOrDefault(x => x.Name == "Unbekannt")
                };
                mappedPackages.Add(mappedPackage);
            }
            mappedPackages.ForEach(x => _appv5Repository.Insert(x));
            _unitOfWork.Save();
        }
        private void SynchronizeCategories(IEnumerable<CustomGroupPrincipal> appv5Packages)
        {
            _appv5Category.Insert(new Category {Id = 40, Name = "Unbekannt"});
            foreach (var package in appv5Packages)
            {
                 if (!String.IsNullOrEmpty(package.Category)&& _appv5Category.Query().FirstOrDefault(cat => cat.Name == package.Category ) == null)
                {
                    
                    _appv5Category.Insert(new Category { Name = package.Category });
                    _unitOfWork.Save();
                }
            }
        }
        public void Synchronize()
        {

            //var defaultDirectoryPath=@"C:\Free-Folder-Icon-PSD.jpg";


            var appvPackages = AdContent.GetAllAppv5Packages();

                var allCategories = _appv5Category.Query().ToList();
                var allPackages = _appv5Repository.Query().ToList();
                var allApplications = _applicationRepository.Query().ToList();

                allApplications.ForEach(x => _applicationRepository.Delete(x));
                allPackages.ForEach(x => _appv5Repository.Delete(x));
                allCategories.ForEach(x => _appv5Category.Delete(x));

                SynchronizeCategories(appvPackages);
                SynchronizePackages(appvPackages);

                SynchronizeApps(appvPackages);

        }

        private void SynchronizeApps(IEnumerable<CustomGroupPrincipal> appvPackages)
        {
            var api_packages = _appvRepository.GetAllPackages();
            foreach (var apiPackage in api_packages)
            {
                //var adPackage =
                  //  appvPackages.FirstOrDefault x =>apiPackage.Entitlements.FirstOrDefault(ent => ent.SidString == x.Sid.ToString() != null));

                
                try
                {
                    var adPackage = appvPackages.FirstOrDefault(ad =>
                    {
                        var firstOrDefault = apiPackage.Entitlements.FirstOrDefault();
                        return firstOrDefault != null && ad.Sid.ToString() == firstOrDefault.SidString;
                    });
                    var packSid = adPackage.Sid.ToString();
                    var dbPackage = _appv5Repository.Query().FirstOrDefault(x => x.PackageSid == packSid);

                    var applications = apiPackage.Applications;

                  //  var request = (HttpWebRequest) WebRequest.Create(apiPackage.PackageUrl);
                    //request.UseDefaultCredentials = true;
                    //request.ContentType = "application/zip";
                    //var stream = request.GetResponse().GetResponseStream();
                    foreach (var application in applications)
                    {
                        var iconAsByteArray = new MemoryStream();
#if !DEBUG
              if (!String.IsNullOrEmpty(application.Icon))
                        {
                            byte[] b;
                            
                            using (var ms = new MemoryStream())
                            {
                                int count;
                                do
                                {
                                    var buf = new byte[1024];
                                    count = stream.Read(buf, 0, 1024);
                                    ms.Write(buf, 0, count);
                                } while (stream.CanRead && count > 0);
                                b = ms.ToArray();
                            }

                            var mem = new MemoryStream(b);

                            using (var archive = new ZipArchive(mem, ZipArchiveMode.Read, true))
                            {
                                var entry = archive.Entries.First();
                                if (entry != null)
                                {
                                    var zipStr = entry.Open();
                                    zipStr.CopyTo(iconAsByteArray);
                                    iconAsByteArray.Seek(0, SeekOrigin.Begin);


                                }

                            }
                            
                        }          
#endif


                        _applicationRepository.Insert(new Appv5Application
                        {
                            Icon = iconAsByteArray.ToArray(),
                            Name = application.Name,
                            LastChanged = DateTime.Now,
                            Package = dbPackage
                        });
                        _unitOfWork.Save();
                    }
                }
                catch
                    (Exception e)
                {

                }

            }
                    //var packageSource = application.Icon.Replace("http://vw-appv5.zdv.uni-mainz.de/appv",
                    //                                    Settings.Default.AppVPathReplacement);
                    //packageSource = packageSource.Replace("/", @"\");
                    //packageSource = packageSource.Replace("%20", "");
                }
               
            }
            

        }

        //private static void SaveIconsFromAppV5ServerToDb(string iconsSource)
        //{

        //    var addedOrUpdatedIcons = 0;
        //    IAppVRepository appCache = new AppVCache(new AppVRepository(iconsSource));
        //    var packagesList = appCache.GetAllPackages();
        //    foreach (var package in packagesList)
        //    {
        //        Console.WriteLine(package.Name);
        //        var pac = package.PackageUrl;
        //        //http://appv5-01/appv
        //        //
        //        var packageSource = pac.Replace("http://vw-appv5.zdv.uni-mainz.de/appv",
        //                                                        Settings.Default.AppVPathReplacement);
        //        packageSource = packageSource.Replace("/", @"\");
        //        packageSource = packageSource.Replace("%20", "");

        //        var zipFile = ZipFile.Open(packageSource, ZipArchiveMode.Read);
        //        var entriesList = zipFile.Entries;
        //        Console.WriteLine(package.Name);
        //        foreach (var app in package.Applications)
        //        {

        //            if (!app.Icon.Equals(""))
        //            {

        //                //Check if the AppIcon is in DB
        //                var path = app.Icon.Split('\\');
        //                var newPath = "Root/" + path[1] + "/" + Uri.EscapeDataString(path[2]);
        //                var entry = entriesList.FirstOrDefault(x => x.FullName.Equals(newPath));
        //                if (entry != null)
        //                {
        //                    var targetMemoryStream = new MemoryStream();
        //                    var copiedStream = new MemoryStream();
        //                    var streamFromZip = entry.Open();
        //                    streamFromZip.CopyTo(copiedStream);
        //                    copiedStream.Seek(0, SeekOrigin.Begin);
        //                    Resize(copiedStream, targetMemoryStream, 64, 64);
        //                    targetMemoryStream.Seek(0, SeekOrigin.Begin);
        //                    var appFromDb = _repository.FindByAppId(app.Id);
        //                    //App isnt in DB
        //                    if (appFromDb == null)
        //                    {
        //                        SaveIconToDb(targetMemoryStream.GetBuffer(), entry.LastWriteTime.DateTime, new Application
        //                        {
        //                            AppId = app.AppId,
        //                            Name = app.Name,
        //                            Description = app.Description,
        //                            Enabled = app.Enabled,
        //                            Id = app.Id,
        //                            Icon = app.Icon,
        //                            Target = app.Target
        //                        });
        //                        ++addedOrUpdatedIcons;
        //                    }
        //                    else
        //                    {
        //                        if (appFromDb.LastChanged.CompareTo(entry.LastWriteTime.DateTime) > 0)
        //                        {
        //                            UpdateIcon(targetMemoryStream.GetBuffer(), entry.LastWriteTime.DateTime, appFromDb, new Application
        //                            {
        //                                AppId = app.AppId,
        //                                Name = app.Name,
        //                                Description = app.Description,
        //                                Enabled = app.Enabled,
        //                                Id = app.Id,
        //                                Icon = app.Icon,
        //                                Target = app.Target
        //                            });
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    Console.WriteLine("{0} icons were added or updated", addedOrUpdatedIcons);
        //}
    

    
