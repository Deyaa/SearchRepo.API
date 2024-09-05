using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using SearchRepo.API.Domain.Services;
using SearchRepo.API.Services;
using SearchRepo.API.Middlewares;
using SearchRepo.API.Filters;

namespace SearchRepo.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            // Add distributed memory cache for session
            services.AddDistributedMemoryCache();
            // Configure session options
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); // Set session timeout
            });

            services.AddScoped<AuthorizeFilter>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddHttpClient();
            services.AddScoped<ISearchService, SearchService>();
            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(Configuration.GetValue<string>("OpenApiInfo:VName"), new OpenApiInfo
                {
                    Version = Configuration.GetValue<string>("OpenApiInfo:Version"),
                    Title = Configuration.GetValue<string>("OpenApiInfo:Title"),
                    Description = Configuration.GetValue<string>("OpenApiInfo:Description"),
                    TermsOfService = new Uri(Configuration.GetValue<string>("OpenApiInfo:TermsOfService")),
                    Contact = new OpenApiContact
                    {
                        Name = Configuration.GetValue<string>("OpenApiInfo:Contact:Name"),
                        Email = Configuration.GetValue<string>("OpenApiInfo:Contact:Email"),
                        Url = new Uri(Configuration.GetValue<string>("OpenApiInfo:Contact:Url"))
                    },
                    License = new OpenApiLicense
                    {
                        Name = Configuration.GetValue<string>("OpenApiInfo:License:Name"),
                        Url = new Uri(Configuration.GetValue<string>("OpenApiInfo:License:Url"))
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. 
        /// Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var basePath = Configuration.GetValue<string>("BasePath");
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer> {
                        new OpenApiServer { 
                            Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}"
                        }
                    };
                });
            });

            var vName = Configuration.GetValue<string>("OpenApiInfo:VName");
            var desc = Configuration.GetValue<string>("OpenApiInfo:Description");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", vName),
                    string.Format("{0}{1}", desc, vName));
            });

            //Handling Errors globaly
            app.ConfigureCustomExceptionMiddleware();

            app.UseSession();

            app.UseRouting();
            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
