using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataAccess
{
    public interface IDataContext
    {
        IDbSet<T> Set<T>() where T : class;
        DbEntityEntry Entry(object o);
        int SaveChanges();
        void Dispose();
    }
}
