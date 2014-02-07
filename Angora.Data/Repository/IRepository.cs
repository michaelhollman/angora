using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Angora.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
        T GetById(object id);
        IEnumerable<T> Find(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        T Single(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        T First(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includeProperties);
        void Delete(T entity);
        void Delete(object id);
        void Insert(T entity);
        void Update(T entity);
        T Refresh(T entity);
    }
}