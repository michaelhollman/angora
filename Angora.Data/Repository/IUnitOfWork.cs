using System;

namespace Angora.Data.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void DiscardChanges();
    }
}