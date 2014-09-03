namespace eTRIKS.Commons.Core.Domain.Model.Base
{
    /// Defines interface for base entity type. All entities in the system must implement this interface.
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public interface IEntity<TPrimaryKey>
    {
        /// Unique identifier for this entity.
        TPrimaryKey OID { get; set; }

    }
}
