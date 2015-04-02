using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDVApps.Services.AD
{
    public class ContextBuilder
    {

            public static PrincipalContext CreateContext(string contextPath)
            {
                var currentDomain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
                var context = new PrincipalContext(ContextType.Domain, currentDomain, contextPath);
                return context;
            }
    }
}
