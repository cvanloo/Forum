using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Forum.Model;
using Forum.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Forum.Controller;
using Ganss.XSS;

namespace Forum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ReSharper disable once MemberCanBePrivate.Global
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
                options.AddPolicy("IsAdmin", p => p.RequireClaim("admin", "True"));
                options.AddPolicy("IsPoster", p => p.RequireClaim("poster", "True"));
            });

            // Database context factory
            var connectionString = Configuration.GetConnectionString("DatabaseConnection");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            services.AddDbContextFactory<Database>(options =>
            {
                options.UseMySql(connectionString, serverVersion)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .ConfigureWarnings(w => w.Throw(
                            // Use to find expensive queries.
                            // Purposefully crash when attempting to execute an expensive query.
                            Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId
                                .MultipleCollectionIncludeWarning)
                        // Ignore "Take/Skip without order by" - false-positive warnings.
                        // False positives: Sometimes ef translates queries without a take or skip into `LIMIT 1`.
                        .Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId
                            .RowLimitingOperationWithoutOrderByWarning)
                    );
            });

            // Smtp Mail service
            services.AddTransient<IMailService, SmtpMailService>();

            // Info Message service
            services.AddSingleton<InfoMessage>();
            
            // Chat service
            services.AddSingleton<IChatService, ChatService>();
            
            // Html sanitizer service
            services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(_ =>
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedAttributes.Add("class");
                sanitizer.AllowedTags.Remove("a"); // disallow links
                return sanitizer;
            });
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
