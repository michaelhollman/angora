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
        private ObjectContext _objectContext;
        private IObjectSet<T> _objectSet;

        public GenericRepository(IObjectContextAdapter context)
        {
            _objectContext = context.ObjectContext;
            _objectSet = context.ObjectContext.CreateObjectSet<T>();
        }


        public IQueryable<T> AsQueryable()
        {
            return _objectSet;
        }

        public IEnumerable<T> GetAll()
        {
            return _objectSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.Where(where);
        }

        public virtual T GetById(long id)
        {
            return _objectSet.Single(t => t.Id == id);
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            return _objectSet.Single(where);
        }

        public T First(Expression<Func<T, bool>> where)
        {
            return _objectSet.First(where);
        }

        public void Delete(T entity)
        {
            _objectSet.DeleteObject(entity);
        }

        public void Insert(T entity)
        {
            _objectSet.AddObject(entity);
        }

        public void Update(T entity)
        {
            _objectSet.Attach(entity);
        }

        public void Refresh()
        {
            // TODO test me!
            _objectContext.Refresh(RefreshMode.StoreWins, _objectSet);
        }

        public void SaveChanges()
        {
            _objectContext.SaveChanges();
        }
    }
}