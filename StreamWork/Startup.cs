using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamWork.Config;
using StreamWork.HelperMethods;
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
            HomeMethods.devEnvironment = env.IsDevelopment();
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.LoginPath = "/Home/Login";
            });

            services.Configure<StorageConfig>(Configuration);

            services.AddTransient<SessionService>();
            services.AddTransient<StorageService>();
            services.AddTransient<ProfileService>();
            services.AddTransient<ScheduleService>();
            services.AddTransient<FollowService>();
            services.AddTransient<ChatService>();
            services.AddTransient<EditService>();
            services.AddTransient<CommentService>();

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
            });
        }
    }
}
