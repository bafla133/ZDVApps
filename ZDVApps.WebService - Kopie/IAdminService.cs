using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ZDVApps.Dtos;

namespace ZDVApps.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAdminWebService" in both code and config file together.
    [ServiceContract]
    public interface IAdminService
    {
        [OperationContract]
        IEnumerable<Dtos.Package> GetAllPackages();

        AdUser GetAdUser(string sid);

        void DepriveUser(string sid,string groupSid);
    }
}
