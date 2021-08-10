using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Forum.Model;
using Microsoft.AspNetCore.Identity;
using Forum.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Forum.Controller;

namespace Forum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServerSideBlazor();
            services.AddRazorPages();

            // Authentication and Authorization services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<AuthenticationStateProvider, AuthenticationStateController>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanAccess", a => a.RequireAuthenticatedUser());
                options.AddPolicy("IsAdmin", p => p.RequireClaim("admin", "true"));
                options.AddPolicy("IsPoster", p => p.RequireClaim("poster", "true"));
            });

            // Database context factory
            string connectionString = Configuration.GetConnectionString("DatabaseConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            services.AddDbContextFactory<Database>(options =>
            {
                options.UseMySql(connectionString, serverVersion)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            });

            // Smtp Mail service
            services.AddTransient<IMailService, SmtpMailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
