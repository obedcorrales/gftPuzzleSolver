using System;
using System.Collections.Generic;

namespace dF.Commons.Services.Data.InMemory
{
    public class DbContext : IDisposable
    {
        protected virtual IDictionary<Type, DbSet> _dbSets { get; private set; } = new Dictionary<Type, DbSet>();

        public DbContext() { }

        public virtual DbSet Set(Type entityType)
        {
            DbSet set;
            if (_dbSets.TryGetValue(entityType, out set))
                return set;

            return null;
        }

        public virtual DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return (DbSet<TEntity>)Set(typeof(TEntity));
        }

        public virtual bool Add<TEntity>(DbSet<TEntity> dbSet) where TEntity : class
        {
            try
            {
                _dbSets.Add(typeof(TEntity), dbSet);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _dbSets = null;
        }
    }
}
