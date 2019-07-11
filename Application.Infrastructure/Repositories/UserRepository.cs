using Application.Core.Interface;
using Application.Core.Models;
using Application.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {

        private DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }
        public async Task<UserModel> CreateUser(UserModel model)
        {
            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                IsActive = model.IsActive,
                IsVerified = model.IsVerified,
                LastName = model.LastName,
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            model.UserId = user.UserId;
            return model;

        }

        public async Task<UserModel> DeleteUser(UserModel model)
        {
            var user = await _context.Set<User>().FindAsync(model.UserId);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<string> GetPasswordHash(string userId)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            return user.PasswordHash;
        }

        public async Task<UserModel> GetUserById(string userId)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new UserModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                LastName = user.LastName,
                UserId = user.UserId,
            };
        }

        public async Task<UserModel[]> GetUsers()
        {
            var query = from user in _context.Set<User>()
                        select new UserModel
                        {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            IsActive = user.IsActive,
                            IsVerified = user.IsVerified,
                            LastName = user.LastName,
                            UserId = user.UserId,
                            Roles = (from role in _context.Set<Role>()
                                     join userroles in _context.Set<UserRole>() on role.RoleId equals userroles.RoleId
                                     where userroles.UserId == user.UserId
                                     select new RoleModel { Name = role.Name, RoleId = role.RoleId }).ToArray() //(Task.Factory.StartNew(()=> GetUserRoles(user.UserId)).Result).Result
                        };

            var users = await query.ToArrayAsync();
            return users;
        }

        public async Task<UserModel[]> FindUser(string search)
        {
            var query = from user in _context.Set<User>()
                        where user.Email.Contains(search)
                        || user.FirstName.Contains(search)
                        || user.LastName.Contains(search)
                        select new UserModel
                        {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            IsActive = user.IsActive,
                            IsVerified = user.IsVerified,
                            LastName = user.LastName,
                            UserId = user.UserId,
                        };

            var users = await query.ToArrayAsync();
            return users;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var query = from user in _context.Set<User>()
                        where user.Email == email
                        select new UserModel
                        {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            IsActive = user.IsActive,
                            IsVerified = user.IsVerified,
                            LastName = user.LastName,
                            UserId = user.UserId,
                        };

            var users = await query.FirstOrDefaultAsync();
            return users;
        }

        public async Task SetEmailConfirmed(string userId)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null)
            {
                return;
            }

            user.IsVerified = true;
            await _context.SaveChangesAsync();
        }

        public async Task SetPasswordHash(string userId, string passwordHash)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null)
            {
                return;
            }

            user.PasswordHash = passwordHash;
            await _context.SaveChangesAsync();
        }

        public async Task<UserModel> UpdateUser(UserModel model)
        {
            var user = await _context.Set<User>().FindAsync(model.UserId);
            if (user == null)
            {
                throw new Exception("Invalid User");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsActive = model.IsActive;
            await _context.SaveChangesAsync();
            return model;
        }


        private async Task<RoleModel[]> GetUserRoles(string userId)
        {
            var query = from role in _context.Set<Role>()
                        join userroles in _context.Set<UserRole>() on role.RoleId equals userroles.RoleId
                        where userroles.UserId == userId
                        select new RoleModel { Name = role.Name, RoleId = role.RoleId };
            var roles = await query.ToArrayAsync();
            return roles;
        }

        public async Task<UserModel> DisableUser(UserModel model)
        {
            var query = await (from user in _context.Set<User>()
                               where user.Email == model.Email
                               select user).FirstOrDefaultAsync();
            query.IsActive = false;
            await _context.SaveChangesAsync();
            return new UserModel
            {
                Email = query.Email,
                FirstName = query.FirstName,
                LastName = query.LastName,
                IsActive = query.IsActive,
                IsVerified = query.IsVerified,
                UserId = query.UserId,
            };
        }

        public async Task<UserModel> EnableUser(UserModel model)
        {
            var query = await (from user in _context.Set<User>()
                               where user.Email == model.Email
                               select user).FirstOrDefaultAsync();
            query.IsActive = true;
            await _context.SaveChangesAsync();
            return new UserModel
            {
                Email = query.Email,
                FirstName = query.FirstName,
                LastName = query.LastName,
                IsActive = query.IsActive,
                IsVerified = query.IsVerified,
                UserId = query.UserId,
            };
        }

        public async Task<UserModel[]> GetUsersInRole(string rolename)
        {
            var query = from user in _context.Set<User>()
                        join userrole in _context.Set<UserRole>() on user.UserId equals userrole.UserId
                        join role in _context.Set<Role>() on userrole.RoleId equals role.RoleId
                        where role.Name == rolename
                        select new UserModel
                        {
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            IsActive = user.IsActive,
                            IsVerified = user.IsVerified,
                            UserId = user.UserId,
                            Roles = user.UserRoles.Select(x => new RoleModel { RoleId = x.RoleId, Name = x.Role.Name }).ToArray()
                        };
            var returnedUser = await query.ToArrayAsync();
            return returnedUser;
        }

        public async Task AddSocialLogin(string userId, SocialLoginModel model)
        {
            var socialLogin = new SocialLogin
            {
                Name = model.Name,
                Key = model.Key,
                SocialLoginId = model.SocialLoginId,
                Provider = model.Provider,
                UserId = model.UserId
                
            };
            _context.Set<SocialLogin>().Add(socialLogin);
            await _context.SaveChangesAsync();
            model.SocialLoginId = socialLogin.SocialLoginId;
        }

        public async Task<UserModel> FindUserBySocialLogin(string loginProvider, string providerKey)
        {
            var query = from socialLogins in _context.Set<SocialLogin>().Include(s => s.User)
                        where socialLogins.Provider == loginProvider
                        where socialLogins.Key == providerKey
                        select socialLogins.User;

            var user = await query.FirstOrDefaultAsync();
            if (user == null) return null;
            return new UserModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                UserId = user.UserId,
            };
        }

        public async Task<SocialLoginModel> GetSocialLogin(string userId, string loginProvider)
        {
            var query = from socialLogins in _context.Set<SocialLogin>()
                        where socialLogins.UserId == userId
                        where socialLogins.Provider == loginProvider
                        select socialLogins;
            var login = await query.FirstOrDefaultAsync();
            if (login == null) return null;
            return new SocialLoginModel
            {
                Name = login.Name,
                Key = login.Key,
                SocialLoginId = login.SocialLoginId,
                Provider = login.Provider,
                UserId = login.UserId
            };
        }

        public async Task<SocialLoginModel[]> GetSocialLogins(string userId)
        {
            var query = from socialLogins in _context.Set<SocialLogin>()
                        where socialLogins.UserId == userId
                        select socialLogins;

            var logins = await query.ToListAsync();
            return logins.Select(login => new SocialLoginModel
            {
                Name = login.Name,
                Key = login.Key,
                SocialLoginId = login.SocialLoginId,
                Provider = login.Provider,
                UserId = login.UserId
            }).ToArray();
        }

        public  async Task RemoveSocialLogin(string userId, string loginProvider, string providerKey)
        {
            var query = from socialLogins in _context.Set<SocialLogin>()
                        where socialLogins.UserId == userId
                        where socialLogins.Provider == loginProvider
                        where socialLogins.Key == providerKey
                        select socialLogins;

            var socialLogin = await query.FirstOrDefaultAsync();
            if (socialLogin == null) throw new Exception("Social Login not Found");
            _context.Set<SocialLogin>().Remove(socialLogin);
            await _context.SaveChangesAsync();
        }
    }
}
