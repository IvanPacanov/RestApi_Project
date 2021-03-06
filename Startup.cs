using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestApi_Dicom.Data;
using RestApi_Dicom.Data.PACSObjectJSON;

namespace RestApi_Dicom
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

            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:5000").AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddMvc(option => option.EnableEndpointRouting = false);
             

            var connStr = Configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<CommanderContext>(opt =>
            //opt.UseSqlServer(
            //    connStr));
            //Configuration.GetConnectionString("CommanderConnection")));
            services.AddTransient<ICommanderPACS, PACSCommanderClass>();
            services.AddHttpContextAccessor();
            services.AddControllers();            
            services.AddScoped<ICommanderPACS, PACSCommanderClass>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
            app.UseMvc();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
