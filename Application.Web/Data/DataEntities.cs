using Application.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Web.Data
{
    public class DataEntities: DbContext
    {
        public DataEntities(DbContextOptions<DataEntities> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SocialLogin> SocialLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Role>().HasData(new Role
            //{
            //    RoleId = new Guid().ToString(),
            //    Name = "Admin",
            //},
            //new Role
            //{
            //    RoleId = new Guid().ToString(),
            //    Name = "User"
            //});
        }
    }
}
