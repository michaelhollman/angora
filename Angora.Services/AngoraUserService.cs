using System.Security.Claims;
using System.Threading.Tasks;
using Angora.Data;
using Angora.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;


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
            // TODO configure this in unity bootstrapper?
            ((UserValidator<AngoraUser>)_userManager.UserValidator).AllowOnlyAlphanumericUserNames = false;
        }

        public async Task<IdentityResult> CreateUser(AngoraUser user)
        {
            return await _userManager.CreateAsync(user);
        }
        public async Task<IdentityResult> UpdateUser(AngoraUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<AngoraUser> FindUser(string username, string password)
        {
            return await _userManager.FindAsync(username, password);
        }

        public AngoraUser FindUser(UserLoginInfo info)
        {
            return _userManager.Find(info);
        }

        public async Task<AngoraUser> FindUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ClaimsIdentity> CreateIdentity(AngoraUser user)
        {
            return await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public async Task<IdentityResult> AddLogin(string id, UserLoginInfo info)
        {
            return await _userManager.AddLoginAsync(id, info);
        }

        public async Task<IdentityResult> RemoveLogin(string id, UserLoginInfo info)
        {
            return await _userManager.RemoveLoginAsync(id, info);
        }


    }
}
