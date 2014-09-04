using System;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Interfaces
{
    public interface IServiceUoW : IDisposable
    {
        void Save();
        IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>;
    }
}
