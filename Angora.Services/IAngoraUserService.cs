using System.Security.Claims;
using System.Threading.Tasks;
using Angora.Data.Models;
using Microsoft.AspNet.Identity;

namespace Angora.Services
{
    public interface IAngoraUserService
    {
        Task<IdentityResult> CreateUser(AngoraUser user);
        Task<IdentityResult> UpdateUser(AngoraUser user);
        Task<AngoraUser> FindUser(string username, string password);
        AngoraUser FindUser(UserLoginInfo info);
        Task<AngoraUser> FindUserById(string id);
        Task<ClaimsIdentity> CreateIdentity(AngoraUser user);
        Task<IdentityResult> AddLogin(string id, UserLoginInfo info);
        Task<IdentityResult> RemoveLogin(string id, UserLoginInfo info);
    }
}
