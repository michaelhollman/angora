using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Angora.Data
{
    // TODO implement IDisposable
    public class GenericRepository<T> : IRepository<T> where T : BaseModel
    {
        private DbSet<T> _dbSet;
        private DbContext _dbContext;

        public GenericRepository(DbContext context)
        {
            _dbContext = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> AsQueryable()
        {
            return _dbSet;
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> where)
        {
            return _dbSet.Where(where);
        }

        public virtual T GetById(long id)
        {
            try
            {
                return _dbSet.Single(t => t.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            return _dbSet.Single(where);
        }

        public T First(Expression<Func<T, bool>> where)
        {
            return _dbSet.First(where);
        }

        public void Delete(T entity)
        {

            _dbSet.Remove(entity);
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}