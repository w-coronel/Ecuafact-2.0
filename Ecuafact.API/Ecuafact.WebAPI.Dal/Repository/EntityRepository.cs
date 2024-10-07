using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using System.Diagnostics;

namespace Ecuafact.WebAPI.Dal.Repository
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class, new()
    {
        protected readonly DbContext DataContext;
        internal DbSet<T> dbSet;

        public Database Database => DataContext.Database;

        public EntityRepository(DbContext entitiesContext)
        {
            DataContext = entitiesContext ?? throw new ArgumentNullException("entitiesContext");
            this.dbSet = DataContext.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            return DataContext.Set<T>();
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return DataContext.Set<T>().Where(predicate);
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return DataContext.Set<T>().Any(predicate);
        }
         
        public virtual IQueryable<T> All
        {
            get
            {
                return GetAll();
            }
        }

        public virtual void Add(T entity)
        {
            //DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            //_entitiesContext.Set<T>().Add(entity);
            dbSet.Add(entity);
        }

        public virtual void AddRange(List<T> entity)
        {           
            dbSet.AddRange(entity);
        }

        public virtual void Edit(T entity)
        {
            //DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            //dbEntityEntry.State = EntityState.Modified;
            dbSet.Attach(entity);            
            DataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void EditRange(List<T> entity)
        {
            //DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            //dbEntityEntry.State = EntityState.Modified;
            dbSet.AttachRange(entity);
            entity.ForEach(p => DataContext.Entry(p).State = EntityState.Modified);           

        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DataContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteRange(List<T> entity)
        {
            //DbEntityEntry dbEntityEntry = _entitiesContext.Entry<T>(entity);
            //dbEntityEntry.State = EntityState.Modified;
            dbSet.AttachRange(entity);
            entity.ForEach(p => DataContext.Entry(p).State = EntityState.Deleted);

        }

        public virtual void Save()
        {
            DataContext.SaveChanges();
        }

        public virtual async Task SaveAsync()
        {
            await DataContext.SaveChangesAsync();
        }

        public IEnumerable<T> ExecSearchesWithStoreProcedure(string query, params object[] parameters)
        {
            return DataContext.Database.SqlQuery<T>(query, parameters);
        }

        public IEntityRepository<TEntity> GetRepository<TEntity>() where TEntity : class, new()
        {
            return new EntityRepository<TEntity>(DataContext);
        }

        public virtual void BulkInsert(List<T> entity)
        {
            var clockInsert = new Stopwatch();
            clockInsert.Start();
            DataContext.BulkInsert(entity);
            clockInsert.Stop();
        }       

        public int? Timeout
        {
            get
            {
                return (DataContext as IObjectContextAdapter)?.ObjectContext?.CommandTimeout;
            }
            set
            {
                var ctx = (DataContext as IObjectContextAdapter)?.ObjectContext;

                if (ctx != null)
                {
                    ctx.CommandTimeout = value;
                }
            }
        }
    }
}
