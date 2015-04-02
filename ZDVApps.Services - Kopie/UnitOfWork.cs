using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDVApps.Common.Contracts;

namespace ZDVApps.Services
{
    public class UnitOfWork: IUnitOfWork
    {
         private readonly IDbContext _context;

        public Database Database
        {
            get { return _context.Database; }
            set { }
        }

        private bool _disposed;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }
    }
    
}
