using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using ZDVApps.Common.Contracts;
using Microsoft.ApplicationServer.Caching;

namespace ZDVApps.Services
{
    public class CacheProvider : ICacheProvider,IDisposable
    {
        private static DataCache _cache;
        public CacheProvider()
        {
            
            //var endPoints = new List<DataCacheServerEndpoint>();
            //Properties.Settings.Default.AppFabricEndPoints.Split(',').ForEach(x=>endPoints.Add
              //  (new DataCacheServerEndpoint(x,Convert.ToInt32(Properties.Settings.Default.AppFabricEndPointsPorts))));
            //ar conf = new DataCacheFactoryConfiguration();


            //conf.Servers = endPoints;
            var factory = new DataCacheFactory();
            _cache = factory.GetDefaultCache();
        }

        public void ResetCache()
        {
            
            _cache.ClearRegion("UsersPackages");
            _cache.ClearRegion("ADUsers");
            _cache.ClearRegion("ADUsers_name");
            _cache.ClearRegion("ADGroups");
            _cache.ClearRegion("AdministrativePackagesRegion");
            _cache.ClearRegion("UserSidsRegion");
            _cache.ClearRegion("ADGroupsRegion");
            _cache.ClearRegion("AppVPackageDependencies");

            _cache.ClearRegion("AllPackagesExtended");
            _cache.ClearRegion("AllPackagesBasic");
        }

        public void Add(string key, object value)
        {
            try
            {
                _cache.Add(key, value);
            }
            catch (DataCacheException e)
            {
                if (e.ErrorCode == DataCacheErrorCode.KeyAlreadyExists)
                {
                    _cache.Remove(key);
                    _cache.Add(key, value);
                }
            }

        }


        public void AddToRegion(string key, object value, TimeSpan expirationTime, string regionName)
        {
            try
            {
                _cache.Add(key, value, expirationTime, regionName);
            }
            catch (DataCacheException e)
            {
                switch (e.ErrorCode)
                {
                    case DataCacheErrorCode.RegionDoesNotExist:
                        _cache.CreateRegion(regionName);
                        _cache.Add(key, value, expirationTime, regionName);
                        break;
                    case DataCacheErrorCode.KeyAlreadyExists:
                        _cache.Remove(key, regionName);
                        _cache.Add(key, value, expirationTime, regionName);
                        break;
                }

            }

        }

        public void AddToRegion(string key, object value, string regionName)
        {
            try
            {
                _cache.Add(key, value, regionName);
            }
            catch (DataCacheException e)
            {
                switch (e.ErrorCode)
                {
                    case DataCacheErrorCode.RegionDoesNotExist:
                        _cache.CreateRegion(regionName);
                        _cache.Add(key, value, regionName);
                        break;
                    case DataCacheErrorCode.KeyAlreadyExists:
                        _cache.Remove(key, regionName);
                        _cache.Add(key, value, regionName);
                        break;
                }

            }

        }

        public object Get(string key, string region)
        {
            try
            {
                return _cache.Get(key, region);
            }
            catch (DataCacheException e)
            {
                if (e.ErrorCode == DataCacheErrorCode.RegionDoesNotExist)
                {
                    CreateRegion(region);
                    return null;
                }
                return null;
            }
        }

        public object this[string key]
        {
            get { return _cache[key]; }
            set { _cache[key] = value; }
        }

        public bool Remove(string key, string groupName)
        {
            try
            {
                return _cache.Remove(key, groupName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CreateRegion(string regionName)
        {
            try
            {
                _cache.CreateRegion(regionName);
            }
            catch (Exception)
            {

            }
        }

        public List<KeyValuePair<string, object>> GetObjectsByRegion(string regionName)
        {
            try
            {
                return _cache.GetObjectsInRegion(regionName).ToList();
            }
            catch (DataCacheException e)
            {
                if (e.ErrorCode == DataCacheErrorCode.RegionDoesNotExist)
                {
                    _cache.CreateRegion(regionName);
                    return null;
                }
                return null;
            }

        }

        public void ClearRegion(string regionName)
        {
            try
            {
                _cache.ClearRegion(regionName);
            }
            catch (DataCacheException e)
            {
                if (e.ErrorCode == DataCacheErrorCode.RegionDoesNotExist)
                {
                    _cache.CreateRegion(regionName);
                }
            }
        }

        public void RemoveRegion(string groupName)
        {
            try
            {
                _cache.Remove(groupName);
            }
            catch (Exception)
            {
            }
        }

        public void ResetObjectTimeOut(string key, TimeSpan expirationTime, string regionName)
        {
            _cache.ResetObjectTimeout(key, expirationTime, regionName);
        }


        private bool _isDisposed;
        //---------------------------------------------------------------------
        protected void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _cache = null;
            }

            _isDisposed = true;
        }
        //---------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
