using Entity;
using Entity.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EF
{
    public class BaseRepository<T> where T: BaseEntity, new()
    {

        protected LOCAL_DBEntities CreateContext()
        {
            return new LOCAL_DBEntities();
        }

        public List<T> FindAll()
        {
            return CreateContext().Set<T>().ToList();
        }

        public List<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            var list = CreateContext().Set<T>().Where(predicate).ToList();

            return list;
        }

        public List<T> FindAll<OT>(Expression<Func<T, bool>> predicate, Func<T, OT> orderSelector, bool isOrderASC = true)
        {
            List<T> list = null;
            if (isOrderASC)
            {
                list = CreateContext().Set<T>().Where(predicate).OrderBy(orderSelector).ToList();
            }
            else
            {
                list = CreateContext().Set<T>().Where(predicate).OrderByDescending(orderSelector).ToList();
            }
            return list;
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var entity = CreateContext().Set<T>().FirstOrDefault(predicate);
            return entity;
        }

        public T FirstOrDefault<OT>(Expression<Func<T, bool>> predicate, Func<T, OT> ordrBySelector)
        {
            var entity = CreateContext().Set<T>().Where(predicate).OrderBy(ordrBySelector).FirstOrDefault();
            return entity;
        }

        public T FirstOrNull(Expression<Func<T, bool>> predicate)
        {
            T entity = null;
            try
            {
                entity = CreateContext().Set<T>().First(predicate);
            }
            catch (Exception ex)
            {

            }
            return entity;
        }

        public T SyncFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            using (var db2 = CreateContext())
            {
                var entity = db2.Set<T>().FirstOrDefault(predicate);
                return entity;
            }
        }

        public bool Add(T entity)
        {
            DbContext dbContext = CreateContext();
            dbContext.Entry<T>(entity).State = EntityState.Added;
            int effectCount = dbContext.SaveChanges();
            return effectCount > 0; 
        }

        public bool ModifyById(T entity)
        {
            DbContext dbContext = CreateContext();
            var oldEntity = dbContext.Set<T>().FirstOrDefault(p => p.ID == entity.ID);
            if (oldEntity == null)
            {
                dbContext.Entry<T>(entity).State = EntityState.Added;
            }
            else
            {
                dbContext.Entry(oldEntity).CurrentValues.SetValues(entity);
            }

            return dbContext.SaveChanges() > 0;
        }

        public bool Modify(Expression<Func<T, bool>> predicate, Action<T> preHandler)
        {
            return ModifyWithResult(predicate, preHandler) != null;
        }

        public T ModifyWithResult(Expression<Func<T, bool>> predicate, Action<T> preHandler)
        {
            using (DbContext dbContext = CreateContext())
            {
                //Todo 需要提升效率

                T oldEntity = null;
                try
                {
                    oldEntity = dbContext.Set<T>().First(predicate);
                }
                catch
                {
                    return null;
                }
                //var oldEntity = FirstOrNull(predicate);
                if (oldEntity != null)
                {
                    preHandler(oldEntity);
                }
                if (dbContext.SaveChanges() == 0)
                {
                    return null;
                }
                return oldEntity;
            }
        }

        public int ModifyNoTracking(T entity)
        {
            using (DbContext dbContext = CreateContext())
            {
                dbContext.Entry<T>(entity).State = EntityState.Modified;
                dbContext.Entry<T>(entity).CurrentValues.SetValues(entity);
                var result = dbContext.SaveChanges();
                return result;
            }
        }

        public void Delete(T entity)
        {
            using (DbContext dbContext = CreateContext())
            {
                dbContext.Entry<T>(entity).State = EntityState.Deleted;
                dbContext.SaveChanges();
            }
        }

        public bool DeleteList(List<T> entityList)
        {
            using (DbContext dbContext = CreateContext())
            {
                entityList.ForEach((entity) =>
                {
                    dbContext.Entry<T>(entity).State = EntityState.Deleted;
                });
                var effectCount = dbContext.SaveChanges();
                return effectCount > 0;
            }
        }

        public int Count()
        {
            using (DbContext dbContext = CreateContext())
            {
                var result = dbContext.Set<T>().Count();
                return result;
            }
        }

        public bool Any()
        {
            var result = CreateContext().Set<T>().Any();

            return result;
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            var result = CreateContext().Set<T>().Any(predicate);

            return result;
        }

        #region SUM

        public decimal? Sum(Expression<Func<T, decimal?>> predicate)
        {
            var result = CreateContext().Set<T>().Sum(predicate);

            return result;
        }

        public int? Sum(Expression<Func<T, int?>> predicate)
        {
            var result = CreateContext().Set<T>().Sum(predicate);

            return result;
        }

        public long? Sum(Expression<Func<T, long?>> predicate)
        {
            var result = CreateContext().Set<T>().Sum(predicate);

            return result;
        }

        #endregion

    }
}
