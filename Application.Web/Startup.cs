using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Core.Interface;
using Application.Core.Models;
using Application.Infrastructure.Repositories;
using Application.Web.Data;
using Application.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Application.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var conn = Configuration.GetConnectionString("DataEntities");
            var migrationAssembly = typeof(DataEntities).GetTypeInfo().Assembly.GetName().Name;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<DbContext, DataEntities>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserStore<UserModel>, UserStore>();
            services.AddScoped<IRoleStore<RoleModel>, RoleStore>();
            services.AddIdentity<UserModel, RoleModel>(o =>
           {
               o.Password.RequireDigit = false;
               o.Password.RequireLowercase = false;
               o.Password.RequireUppercase = false;
               o.Password.RequireNonAlphanumeric = false;
           }).AddDefaultTokenProviders();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<DataEntities>(o => o.UseSqlServer(conn, p => p.MigrationsAssembly(migrationAssembly)));


            var token = Configuration.GetSection("TokenOptions").Get<Core.Models.TokenOptions>();
            var secret = Encoding.UTF8.GetBytes(token.Secret);

            services.AddSingleton(token);


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
           {
               o.RequireHttpsMetadata = false;
               o.SaveToken = true;
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidIssuer = token.Issuer,
                   ValidAudience = token.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(secret),
                   ClockSkew = TimeSpan.Zero
               };

           }).AddCookie("cookie");

            var corsOriginString = Configuration.GetValue<string>("CorsOrigins");
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    var corsOrigins = corsOriginString.Split(",");
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                           .WithOrigins(corsOrigins)
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .AllowAnyHeader()
                           .Build();
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseStatusCodePages(async context =>
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;
                var path = request.Path.Value ?? "";

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    response.Redirect("/Account/Login");
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
