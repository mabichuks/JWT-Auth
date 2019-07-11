using Application.Core.Interface;
using Application.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Web.Security
{
    public class UserStore : IUserStore<UserModel>, IUserPasswordStore<UserModel>, IUserEmailStore<UserModel>, IUserClaimsPrincipalFactory<UserModel>, IUserClaimStore<UserModel>, IUserRoleStore<UserModel>, IUserLoginStore<UserModel>
    {
        private readonly IUserRepository _user;
        private readonly IRoleRepository _role;

        public UserStore(IUserRepository userRepo, IRoleRepository roleRepo)
        {
            _user = userRepo;
            _role = roleRepo;
        }

        public Task AddClaimsAsync(UserModel user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(UserModel user, CancellationToken cancellationToken)
        {
            try
            {
                await _user.CreateUser(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = "IdError01", Description = ex.Message });
            }
        }

        public Task<UserModel[]> GetAllUsers(int pageNumber, int pageSize) => _user.GetUsers();

        public Task<UserModel[]> FindUser(string search, int pageNumber, int pageSize) => _user.FindUser(search);

        public async Task<ClaimsPrincipal> CreateAsync(UserModel user)
        {
            await Task.Yield();
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, user.LastName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.UserId ?? ""),
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

        public async Task<IdentityResult> DeleteAsync(UserModel user, CancellationToken cancellationToken)
        {
            try
            {
                await _user.DeleteUser(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                var error = new IdentityError { Code = "IDError05", Description = ex.Message };
                return IdentityResult.Failed(error);
            }
        }

        public void Dispose()
        {
            //Do Nothing
        }

        public async Task<UserModel> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = await _user.GetUserByEmail(normalizedEmail);
            if (user != null) user.Roles = await _role.GetUserRoles(user.UserId);
            return user;
        }

        public async Task<UserModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _user.GetUserById(userId);
            if (user != null) user.Roles = await _role.GetUserRoles(user.UserId);
            return user;
        }

        public async Task<UserModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {

            var user = await _user.GetUserByEmail(normalizedUserName);
            if (user != null) user.Roles = await _role.GetUserRoles(user.UserId);
            return user;

        }



        public async Task<IList<Claim>> GetClaimsAsync(UserModel user, CancellationToken cancellationToken)
        {
            return (await CreateAsync(user)).Claims.ToList();
        }

        public Task<string> GetEmailAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.IsVerified);
        }

        public Task<string> GetNormalizedEmailAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<string> GetNormalizedUserNameAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<string> GetPasswordHashAsync(UserModel user, CancellationToken cancellationToken)
        {
            return _user.GetPasswordHash(user.UserId);
        }

        public Task<string> GetUserIdAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserId);
        }

        public Task<string> GetUserNameAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<IList<UserModel>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(UserModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task RemoveClaimsAsync(UserModel user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(UserModel user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(UserModel user, string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(UserModel user, bool confirmed, CancellationToken cancellationToken)
        {
            return _user.SetEmailConfirmed(user.UserId);
        }

        public Task SetNormalizedEmailAsync(UserModel user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetNormalizedUserNameAsync(UserModel user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(UserModel user, string passwordHash, CancellationToken cancellationToken)
        {
            return _user.SetPasswordHash(user.UserId, passwordHash);
        }

        public Task SetUserNameAsync(UserModel user, string userName, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public async Task<IdentityResult> UpdateAsync(UserModel user, CancellationToken cancellationToken)
        {
            try
            {
                await _user.UpdateUser(user);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = "IdError02", Description = ex.Message });
            }
        }

        public async Task AddToRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _role.GetRoleByName(roleName);
            if (role == null) throw new Exception("Invalid Role Name");
            await _role.AddUserToRole(user.UserId, role.RoleId);
        }

        public async Task RemoveFromRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
        {
            var role = await _role.GetRoleByName(roleName);
            if (role == null) throw new Exception("Invalid Role Name");
            await _role.RemoveUserFromRole(user.UserId, role.RoleId);
        }

        public async Task<IList<string>> GetRolesAsync(UserModel user, CancellationToken cancellationToken)
        {
            var roles = await _role.GetUserRoles(user.UserId);
            return roles.Select(r => r.Name).ToList();
        }

        public Task<bool> IsInRoleAsync(UserModel user, string roleName, CancellationToken cancellationToken)
        {
            return _role.IsUserInRole(user, roleName);
        }

        public async Task<IList<UserModel>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var user = await _user.GetUsersInRole(roleName);
            return user;
        }

        public async Task AddLoginAsync(UserModel user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var sociallogin = await _user.GetSocialLogin(user.UserId, login.LoginProvider);
            if (sociallogin == null)
            {
                sociallogin = ToSocialLogin(login);
                await _user.AddSocialLogin(user.UserId, sociallogin);
            }
        }

        public async Task RemoveLoginAsync(UserModel user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            await _user.RemoveSocialLogin(user.UserId, loginProvider, providerKey);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(UserModel user, CancellationToken cancellationToken)
        {
            var socialLogins = await _user.GetSocialLogins(user.UserId);
            return socialLogins.Select(ToLoginInfo).ToList();
        }

        public async Task<UserModel> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return await _user.FindUserBySocialLogin(loginProvider, providerKey);
        }

        private SocialLoginModel ToSocialLogin(UserLoginInfo loginInfo)
        {
            if (loginInfo == null) return null;
            return new SocialLoginModel
            {
                Name = loginInfo.ProviderDisplayName,
                Key = loginInfo.ProviderKey,
                Provider = loginInfo.LoginProvider,
            };
        }

        private UserLoginInfo ToLoginInfo(SocialLoginModel socialLogin)
        {
            if (socialLogin == null) return null;
            return new UserLoginInfo(socialLogin.Provider, socialLogin.Key, socialLogin.Name);
        }
    }

}
