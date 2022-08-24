using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Net.Mail;
using TylerHendricks_Core.Models;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Repo.IdentityContext;
using TylerHendricks_Repo.Services;
using TylerHendricks_Web.Claim;
using TylerHendricks_Web.Filters;

namespace TylerHendricks_Web
{
    public class Startup
    {
        #region Private field initializations
        private readonly IWebHostEnvironment _env;
        #endregion
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string iisEnv = Configuration.GetConnectionString("Environment");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbString")));

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddControllersWithViews(Option =>
            {
                var Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                Option.Filters.Add(new AuthorizeFilter(Policy));
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ".ToolsAppSession";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(11);
                options.LoginPath = "/";
                options.LogoutPath = "/";
                options.SlidingExpiration = true;
                options.Cookie.Name = ".ToolsApp";
            });


            #region Fulent Email Config

            //Fluent Email through SMTP Server
            var From = Configuration.GetSection("EmailConfiguration")["From"];
            var EmailSender = Configuration.GetSection("EmailConfiguration")["FromMailSender"];
            var Port = Convert.ToInt32(Configuration.GetSection("EmailConfiguration")["Port"]);
            var Credentail = Configuration.GetSection("EmailConfiguration")["FromMailPassword"];
            var SmtpServer = Configuration.GetSection("EmailConfiguration")["SmtpClient"];


            services.AddFluentEmail(EmailSender, From)
                .AddRazorRenderer()
                .AddSmtpSender(new SmtpClient(SmtpServer)
                {
                    UseDefaultCredentials = false,
                    Port = Port,
                    Credentials = new NetworkCredential(EmailSender, Credentail),
                    EnableSsl = true
                });

            #endregion

            #region [AppConfig]
            services.AddSingleton(Configuration.GetSection("ResourcesConfig").Get<ResourcesConfig>());

            if (iisEnv == "Production")
            {
                services.AddSingleton(Configuration.GetSection("EnvironmentalResource").GetSection("Production").Get<EnvironmentalResource>());
            }
            else
            {
                services.AddSingleton(Configuration.GetSection("EnvironmentalResource").GetSection("Development").Get<EnvironmentalResource>());
            }

            #endregion
            services.AddScoped<IRepositoryCollection, RepositoryCollection>();
            services.AddScoped<IUsers, Users>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailProvider, EmailProvider>();
            services.AddTransient<CustomLoginedUserAuthentication, CustomLoginedUserAuthentication>();
            services.AddControllersWithViews();
            // Runtime compilation will be executed only Development env
            if (_env.IsDevelopment())
            {
                services.AddRazorPages().AddRazorRuntimeCompilation();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseExceptionHandler("/Error");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "AdminRoute", pattern: "{area=AdminPortal}/{controller=Home}/{action=Login}/{id?}");
                endpoints.MapControllerRoute(name: "AreaRoute", pattern: "{area=PhysicianPortal}/{controller=Home}/{action=Login}/{id?}");
                endpoints.MapControllerRoute(name: "default", pattern: "{area=PatientPortal}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
