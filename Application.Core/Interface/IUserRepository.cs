using Application.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Interface
{
    public interface IUserRepository
    {
        //Task<List<PermissionModel>> GetUserClaims(string userid);
        Task<UserModel> GetUserByEmail(string email);
        Task<UserModel[]> GetUsers();
        Task<UserModel[]> FindUser(string search);
        Task<UserModel> GetUserById(string userId);
        Task<UserModel> CreateUser(UserModel user);
        Task<UserModel[]> GetUsersInRole(string rolename);
        Task<UserModel> DeleteUser(UserModel model);
        Task<UserModel> DisableUser(UserModel model);
        Task<UserModel> EnableUser(UserModel model);
        Task<UserModel> UpdateUser(UserModel user);
        Task<string> GetPasswordHash(string userId);
        Task SetEmailConfirmed(string userId);
        Task SetPasswordHash(string userId, string passwordHash);

        //Social Logins
        Task AddSocialLogin(string userId, SocialLoginModel socialLogin);
        Task<UserModel> FindUserBySocialLogin(string loginProvider, string providerKey);
        Task<SocialLoginModel> GetSocialLogin(string userId, string loginProvider);
        Task<SocialLoginModel[]> GetSocialLogins(string userId);
        Task RemoveSocialLogin(string userId, string loginProvider, string providerKey);
    }
}
