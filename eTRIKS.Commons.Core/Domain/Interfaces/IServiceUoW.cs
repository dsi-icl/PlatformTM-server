﻿using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface IServiceUoW : IDisposable
    {
        string Save();
        IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>;

        void AddClassMap(string fieldname, string propertyName);

        void setSDTMentityDescriptor(SdtmEntityDescriptor descriptor);
        IUserRepository<TEntity,TResult> GetUserRepository<TEntity,TResult>();

    }
}
