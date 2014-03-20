using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Angora.Data.Models;
using Microsoft.AspNet.Identity;

namespace Angora.Services
{



    [ServiceContract]
    public interface IAngoraUserService
    {
        [OperationContract]
        Task<bool> CreateUser(AngoraUser user);
        [OperationContract]
        Task<bool> UpdateUser(AngoraUser user);
        [OperationContract]
        Task<AngoraUser> FindUser(string username, string password);
        [OperationContract]
        Task<AngoraUser> FindUser(UserLoginInfo info);
        [OperationContract]
        Task<AngoraUser> FindUserById(string id);
    }
}
