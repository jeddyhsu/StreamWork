﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamWork.Config;
using StreamWork.Hubs;
using StreamWork.Services;

namespace StreamWork
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("Config/storageconfig.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            CookieService.devEnvironment = env.IsDevelopment();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSignalR();
            services.AddSession();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.LoginPath = "/Home/SignIn";
            })
            .AddGoogle(options =>
            {
                options.ClientId = "200781052449-1vbbl8k9t6g2hr3hd5c2ve8natdjsk9s.apps.googleusercontent.com";
                options.ClientSecret = "5pJv9d7UACsWWPkWKMjb-sTb";
            });


            services.Configure<StorageConfig>(Configuration);

            services.AddTransient<ChatService>();
            services.AddTransient<CommentService>();
            services.AddTransient<EditService>();
            services.AddTransient<EncryptionService>();
            services.AddTransient<FollowService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<ScheduleService>();
            services.AddTransient<SearchService>();
            services.AddTransient<CookieService>();
            services.AddTransient<CookieService>();
            services.AddTransient<StorageService>(); // Transient means it creates a new instance every time it's needed
            services.AddTransient<StreamService>(); // You should use Transient by default
            services.AddTransient<NotificationService>();
            services.AddSingleton<SubjectService>(); // Singleton creates a shared instance the first time it's needed

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddRazorPages().AddRazorRuntimeCompilation().AddRazorOptions(options =>
            {
                options.PageViewLocationFormats.Add("/Pages/Partials/{0}.cshtml");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapRazorPages();
                endpoint.MapControllers();
                endpoint.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
