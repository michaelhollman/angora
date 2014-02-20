using System;
using Angora.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Data
{
    public static class RepositoryFactory
    {
        public static GenericRepository<T> NewRepository<T>() where T : BaseModel
        {
            return new GenericRepository<T>(new AngoraDbContext());
        }

        // putting this here for now. look into moving elsewhere, not sure where yet.
        public static UserManager<AngoraUser> NewUserManager()
        {
            return new UserManager<AngoraUser>(new UserStore<AngoraUser>(new AngoraDbContext()));
        }

    }
}
