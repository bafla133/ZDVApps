using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ZDVApps.Common.Contracts.WebServices;
using ZDVApps.DataTransfer.Contracts;

namespace ZDVApps.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SynchronizationService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SynchronizationService.svc or SynchronizationService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [DataContract]
    public class SynchronizationWebService : ISynchronizationWebService
    {
        private readonly ISynchronizationService _synchronizationService;
        public SynchronizationWebService(ISynchronizationService synchronizationService)
        {
            _synchronizationService = synchronizationService;
        }

        public void Syhncronize()
        {
            _synchronizationService.Synchronize();
        }
    }
}
