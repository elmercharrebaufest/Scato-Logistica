using Molinos.Scato.API.Infrastructure.Repository.Interfaces.Core;
using Molinos.Scato.API.CrossCutting.IoC;
using  Molinos.Scato.API.WebAPI.Installers.Core;
using Molinos.Scato.API.Infrastructure.Repository.Implementation.Core;

namespace  Molinos.Scato.API.WebAPI
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
            services.AddControllers();
            services.AddControllersWithViews().
               AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   options.JsonSerializerOptions.PropertyNamingPolicy = null;
               });
            //Only for migration generation. comment after use
            //services.AddDbContext<IdentityDataContext>(options => options.UseNpgsql("DefaultConnection"));
            services.InstallServicesInAssembly(Configuration);
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors();
            IoCContainer.SetServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //TODO Add https
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(x => x
                 .AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.InjectStylesheet("/swagger/header.css");
            });
        }

        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new ApplicationModule());
        //}

        //public class ApplicationModule : Autofac.Module
        //{
        //    protected override void Load(ContainerBuilder builder)
        //    {
        //        IoCContainer.Initialize(builder);
        //    }
        //}
    }
}
