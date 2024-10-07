using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Ecuafact.WebAPI.Domain.Repository
{
    public interface IEntityRepository<T> where T : class, new()
    {
        Database Database { get; }
        IQueryable<T> All { get; }
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        bool Exists(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        void Save();
        void BulkInsert(List<T> entity);
        void AddRange(List<T> entity);
        void EditRange(List<T> entity);
        void DeleteRange(List<T> entity);

        IEnumerable<T> ExecSearchesWithStoreProcedure(string query, params object[] parameters);

        IEntityRepository<TEntity> GetRepository<TEntity>()
                         where TEntity : class, new();
        /// <summary>
        /// Gets or sets the timeout value, in seconds, for all object context operations.
        ///     A null value indicates that the default value of the underlying provider will
        ///     be used.
        /// </summary>
        int? Timeout { get; set; }

    }
}
