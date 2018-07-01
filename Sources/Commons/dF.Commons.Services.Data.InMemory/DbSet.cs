using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using dF.Commons.Core.Helpers;
using dF.Commons.Exceptions.Extensions;
using dF.Commons.Exceptions.Resources;
using dF.Commons.Helpers;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Models.Globals.Extensions;
using dF.Commons.Proxies.DirtyPropertiesTrackerProxy;

namespace dF.Commons.Services.Data.InMemory
{
    public class DbSet
    {
        public Type EntityType { get; protected set; }

        protected DbSet() { }
    }

    public class DbSet<TEntity> : DbSet where TEntity : class
    {
        IList<TEntity> _local;

        public virtual IList<TEntity> Local
        {
            get { return DeepCloning.Copy(_local); }
        }

        public virtual int Count => _local.Count;

        protected IList<PropertyDescriptor> _keyMembers;

        public DbSet() : base()
        {
            _local = new List<TEntity>();
            _keyMembers = new List<PropertyDescriptor>();
            EntityType = typeof(TEntity);
        }

        public DbSet(IList<TEntity> seedData) : this()
        {
            andSeed(seedData);
        }

        #region CRUD
        #region Add
        public virtual Result<TEntity> Add(TEntity entity)
        {
            try
            {
                List<object> parentKeys = new List<object>();
                var iKey = 0;

                if (entity is IDirtyPropertiesTracker)
                {
                    var modifiedProperties = (entity as IDirtyPropertiesTracker).GetDirtyPropertiesNameList();

                    foreach (var key in _keyMembers)
                    {
                        iKey++;
                        if (modifiedProperties.Count(p => p == key.Name) == 0)
                        {
                            var id = key.GetNextId();
                            if (id.IsSuccess)
                            {
                                key.AssignId(id.Value, iKey < _keyMembers.Count, parentKeys.ToArray());
                                parentKeys.Add(id.Value);
                                key.Property.SetValue(entity, Convert.ChangeType(id.Value, key.Type));
                            }
                        }
                        else
                        {
                            var id = key.Property.GetValue(entity);

                            key.AssignId(id, iKey < _keyMembers.Count, parentKeys.ToArray())
                                .OnSuccess(() => parentKeys.Add(id))
                                .OnFailureThrowException();
                        }
                    }
                }
                else
                {
                    foreach (var key in _keyMembers)
                    {
                        iKey++;
                        if (TypeHelper.IsNull<TEntity>(key.Property, ref entity))
                        {
                            var id = iKey == _keyMembers.Count ? key.GetNextId() : Result.Ok(TypeDefaults.Default(key.Type));
                            if (id.IsSuccess)
                            {
                                key.AssignId(id.Value, iKey < _keyMembers.Count, parentKeys.ToArray());
                                parentKeys.Add(id.Value);
                                key.Property.SetValue(entity, Convert.ChangeType(id.Value, key.Type));
                            }
                        }
                        else
                        {
                            var id = key.Property.GetValue(entity);

                            key.AssignId(id, iKey < _keyMembers.Count, parentKeys.ToArray())
                                .OnSuccess(() => parentKeys.Add(id))
                                .OnFailureThrowException();
                        }
                    }
                }

                _local.Add(entity);
                return Result.Ok(entity);
            }
            catch (Exception e)
            {
                return Result.Fail<TEntity>(e);
            }
        }
        #endregion

        #region Update
        public virtual Result Update(TEntity entity)
        {
            try
            {
                // Build keyValues
                var keyValues = new object[_keyMembers.Count];

                for (int i = 0; i < _keyMembers.Count; i++)
                {
                    keyValues[i] = _keyMembers[i].Property.GetValue(entity);
                }

                // Get Entity
                var dbEntityResult = Find(false, keyValues);
                if (dbEntityResult.IsFailure)
                    return dbEntityResult;

                var updateResult = EntityPatcher<TEntity>.Patch(entity, dbEntityResult.Value);

                if (updateResult.IsSuccess)
                {
                    var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    foreach (var property in properties)
                    {
                        if (property.CanRead && property.CanWrite)
                            property.SetValue(dbEntityResult.Value, property.GetValue(updateResult.Value));
                    }
                }
                else
                    return updateResult;

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }
        }
        #endregion

        #region Delete
        public virtual Result Delete(TEntity entity)
        {
            try
            {
                // Build keyValues
                var keyValues = new object[_keyMembers.Count];

                for (int i = 0; i < _keyMembers.Count; i++)
                {
                    keyValues[i] = _keyMembers[i].Property.GetValue(entity);
                }

                // Get Entity
                var dbEntityResult = Find(false, keyValues);
                if (dbEntityResult.IsFailure)
                    return dbEntityResult;

                // Remove Entity
                _local.Remove(dbEntityResult.Value);

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }
        }
        #endregion

        #region Find
        private Result<Expression> GetFindExpression(params object[] keyValues)
        {
#if (TEST)
            var watch = Stopwatch.StartNew();
#endif
            if (keyValues == null)
                return Result.Fail<Expression>(string.Format(ExceptionMessages.ArgumentException_ProvideValue, "Key"), ErrorType.ArgumentNull);

            if (keyValues.Count() != _keyMembers.Count)
                return Result.Fail<Expression>("keyValues must match the number of fields in the entity key", ErrorType.InvalidOperation);

            // Func<TEntity, bool> f = (TEntity e) => e.keyMember = keyValues;
            Expression expr = null;
            var param = Expression.Parameter(typeof(TEntity), "e");
            for (int i = 0; i < _keyMembers.Count; i++)
            {
                var keyMember = Expression.PropertyOrField(param, _keyMembers[i].Name);
                var keyValue = Expression.Constant(keyValues[i]);
                var comparison = Expression.Equal(keyMember, keyValue);
                if (expr == null)
                    expr = comparison;
                else
                    expr = Expression.And(expr, comparison);
            }

            if (expr != null)
                expr = Expression.Lambda<Func<TEntity, bool>>(expr, param);
#if (TEST)
            watch.Stop();
            Console.WriteLine(string.Format("Ticks Getting Find Expression: {0}", watch.ElapsedTicks));
#endif

            return Result.Ok(expr);
        }

        public virtual Result<TEntity> Find(params object[] keyValues)
        {
            return Find(true, keyValues);
        }

        private Result<TEntity> Find(bool clone, params object[] keyValues)
        {
            try
            {
                var expressionResult = GetFindExpression(keyValues);

                if (expressionResult.IsFailure)
                    return Result.Fail<TEntity>(expressionResult.Error, expressionResult.ErrorType);

                return Find(expressionResult.Value, clone);
            }
            catch (Exception e)
            {
                return Result.Fail<TEntity>(e);
            }
        }

        private Result<TEntity> Find(Expression expression, bool clone = false)
        {
#if (TEST)
            var watch = Stopwatch.StartNew();
#endif
            var entity = _local.AsQueryable<TEntity>().Where<TEntity>((Expression<Func<TEntity, bool>>)expression).FirstOrDefault();

            if (entity == null)
                return Result.Fail<TEntity>(string.Format(ExceptionMessages.EntityNotFoundException_NoRecordFoundByProvidedID, nameof(TEntity)), ErrorType.EntityNotFound);

#if (TEST)
            watch.Stop();
            Console.WriteLine(string.Format("Ticks Querying through Find Expression: {0}", watch.ElapsedTicks));
#endif
            if (clone)
                return Result.Ok(DeepCloning.Copy(entity));
            else
                return Result.Ok(entity);
        }
        #endregion

        #region Where
        public virtual Result<IQueryable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var entities = _local.AsQueryable<TEntity>().Where<TEntity>(predicate);

                return Result.Ok(entities);
            }
            catch (Exception e)
            {
                return Result.Fail<IQueryable<TEntity>>(e);
            }
        }
        #endregion
        #endregion

        #region Initializers
        public virtual DbSet<TEntity> andKey<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            //Expression<Func<object, object, bool>> keyComparer = (e, p) => e == p;

            var memberExpression = (MemberExpression)keySelector.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;

            var result = PropertyDescriptor.Create(propertyInfo, true)
                .OnSuccess((PropertyDescriptor p) => _keyMembers.Add(p))
                .OnFailureThrowException();

            //if (result.IsSuccess)
            //    _keyMembers.Add(result.Value);

            //if (result.IsFailure)
            //    throw new Exception(result.Error);

            return this;
        }

        public virtual DbSet<TEntity> andSeed(IList<TEntity> seedData)
        {
            foreach (var seed in seedData)
            {
                Add(seed);
            }

            return this;
        }

        public static DbSet<TEntity> withKey<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            return new DbSet<TEntity>().andKey(keySelector);
        }
        #endregion

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return Local.AsQueryable();
        }

        public virtual IEnumerable<TEntity> AsEnumerable()
        {
            return Local.AsEnumerable();
        }

        public virtual List<TEntity> ToList()
        {
            return (List<TEntity>)Local;
        }

        public virtual TEntity[] ToArray()
        {
            return Local.ToArray();
        }

        public static implicit operator List<TEntity>(DbSet<TEntity> dbSet)
        {
            return dbSet.ToList();
        }
    }
}
