using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Contracts;
using ZDVApps.Model.DB;

namespace ZDVApps.Services.DBRepositories
{
  
    public class Appv5PackagesRepository : Repository<Appv5Package>
    {
        public Appv5PackagesRepository(IDbContext context)
            : base(context)
        {

        }

        
    }
}
