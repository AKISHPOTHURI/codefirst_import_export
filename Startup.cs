namespace code_first
{
    using code_first.IRepository;
    using code_first.IService;
    using code_first.Mapper;
    using code_first.Models;
    using code_first.Repository;
    using code_first.Service;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
            services.AddDbContext<ProjectDbContext>(x => x.UseSqlServer(Configuration.GetConnectionString("ConStr")));
            services.AddCors(p => p.AddPolicy("CodeFirstFrontEnd", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));
            services.AddAutoMapper(typeof(ApplicationMapper));


            //services
            services.AddScoped<IEmployeeService, EmployeeService>();

            //Repository
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "code_first API",
                    Description = "code_first API",
                    Contact = new OpenApiContact
                    {
                        Name = "Code First",
                        Email = string.Empty,
                        //Url = new Uri("https://www.innovasolutions.com/"),
                    },
                });

                //c.AddSecurityDefinition("token", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    Description = "Please enter token",
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    Scheme = "Bearer"
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type=ReferenceType.SecurityScheme,
                //                Id="token"
                //            }
                //        },
                //        new string[]{}
                //    }
                //});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseCors("CodeFirst");

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "code_first API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
