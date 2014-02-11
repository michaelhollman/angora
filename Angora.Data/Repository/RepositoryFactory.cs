using System;

namespace Angora.Data
{
    public static class RepositoryFactory
    {
        public static GenericRepository<T> NewRepository<T>() where T : BaseModel
        {
            return new GenericRepository<T>(new AngoraContext());
        }

    }
}
