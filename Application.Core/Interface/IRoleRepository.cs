using Application.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Interface
{
    public interface IRoleRepository
    {
        Task<RoleModel> CreateRole(RoleModel role);
        Task DeleteRole(RoleModel role);
        Task<RoleModel> GetRoleById(string roleId);
        Task<RoleModel> GetRoleByName(string roleName);
        Task UpdateRole(RoleModel role);
        Task<RoleModel[]> GetRoles();
        RoleModel[] GetRolesQuerable();
        Task AddUserToRole(string userId, string roleId);
        Task<UserModel> RemoveUserFromRole(string userId, string roleId);
        Task<RoleModel[]> GetUserRoles(string userId);
        Task<bool> IsUserInRole(UserModel user, string roleName);

        Task<RoleModel[]> ByUserId(string clientId, string userId);

        Task<UserRoleModel> GetUserRole(string userId, string roleId);
    }
}
