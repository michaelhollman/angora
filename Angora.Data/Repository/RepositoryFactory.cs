using System;
using System.Collections.Generic;
using Angora.Data.Models;

namespace Angora.Data.Repository
{
    public static class RepositoryFactory
    {
        private static readonly AngoraDbContext Context;

        static RepositoryFactory()
        {
            Context = new AngoraDbContext();
        }

        public static GenericRepository<T> NewRepository<T>() where T : BaseModel
        {
            return new GenericRepository<T>(Context);
        }

    }
}
