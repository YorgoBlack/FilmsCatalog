using FilmsCatalog.Data;
using FilmsCatalog.Mappings;
using FilmsCatalog.Models;
using FilmsCatalog.Services;
using LiteX.Storage.FileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace FilmsCatalog
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
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddHttpContextAccessor();
            
            var bs_config = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json")
                            .Build()
                            .GetSection(nameof(BlobStorageSettings))
                            .Get<BlobStorageSettings>();
            string fs_storage_path = System.IO.Path.IsPathRooted(bs_config.LocalPath) ? bs_config.LocalPath
                : System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), bs_config.LocalPath);
            if( !System.IO.Directory.Exists(fs_storage_path) )
            {
                System.IO.Directory.CreateDirectory(fs_storage_path);
            }
            services.AddLiteXFileSystemStorageService(new FileSystemStorageConfig()
            {
                Directory = fs_storage_path,
                EnableLogging = false
            });

            services.AddScoped(typeof(IFilmsCatalogService), typeof(FilmsCatalogService));
            services.AddAutoMapper(typeof(FilmMap));
            services.AddDatabaseDeveloperPageExceptionFilter();            
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string baseDir = env.ContentRootPath;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(baseDir, "App_Data"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
