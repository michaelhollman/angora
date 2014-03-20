using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data;
using Angora.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Services
{

    // TODO: move as much of the current AccountController logic into here as possible/practical

    public class AngoraUserService : ServiceBase, IAngoraUserService
    {
        private UserManager<AngoraUser> _userManager;

        public AngoraUserService()
            : this(new UserManager<AngoraUser>(new UserStore<AngoraUser>(new AngoraDbContext()))) { }

        public AngoraUserService(UserManager<AngoraUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CreateUser(AngoraUser user)
        {
            var r = await _userManager.CreateAsync(user);
            return r.Succeeded;
        }
        public async Task<bool> UpdateUser(AngoraUser user)
        {
            var r = await _userManager.UpdateAsync(user);
            return r.Succeeded;
        }

        public async Task<AngoraUser> FindUser(string username, string password)
        {
            return await _userManager.FindAsync(username, password);
        }

        public async Task<AngoraUser> FindUser(UserLoginInfo info)
        {
            return await _userManager.FindAsync(info);
        }

        public async Task<AngoraUser> FindUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }



    }
}
