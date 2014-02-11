using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Angora.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> where);
        T GetById(long id);
        T Single(Expression<Func<T, bool>> where);
        T First(Expression<Func<T, bool>> where);
        void Delete(T entity);
        void Insert(T entity);
        void Update(T entity);
        void Refresh();

        // unit of work functionality
        void SaveChanges();
    }
}