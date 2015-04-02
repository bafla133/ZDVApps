using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Contracts;
using ZDVApps.Model.DB;

namespace ZDVApps.Services.DBRepositories
{
  
    public class Appv5CategoryRepository : Repository<Category>
    {
        public Appv5CategoryRepository(IDbContext context)
            : base(context)
        {

        }

        
    }
}
