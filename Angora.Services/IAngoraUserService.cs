using System.Security.Claims;
using System.ServiceModel;
using System.Threading.Tasks;
using Angora.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Angora.Services
{
    [ServiceContract]
    public interface IAngoraUserService
    {
        [OperationContract]
        Task<IdentityResult> CreateUser(AngoraUser user);

        [OperationContract]
        Task<IdentityResult> UpdateUser(AngoraUser user);

        [OperationContract]
        Task<AngoraUser> FindUser(string username, string password);

        [OperationContract]
        AngoraUser FindUser(UserLoginInfo info);

        [OperationContract]
        Task<AngoraUser> FindUserById(string id);

        [OperationContract]
        Task<ClaimsIdentity> CreateIdentity(AngoraUser user);
        
        [OperationContract]
        Task<IdentityResult> AddLogin(string id, UserLoginInfo info);
    }
}
