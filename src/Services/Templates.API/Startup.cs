using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Extensions;
using Utils;
using Boxed.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Templates.API.Infrastructure.Migrations;
using Serilog;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;
using System.Text;
using API.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.IdentityModel.Tokens;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;

namespace Templates.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            // Init Serilog configuration
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                .MinimumLevel.Information()
                .CreateLogger();
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(x => x.Filters.Add(typeof(LogFilter))).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<TaskJobSynchronizedMerchant>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(TaskJobSynchronizedMerchant),
                cronExpression: "0 0 */6 ? * *")); // run every 5 seconds

            services.AddHostedService<QuartzHostedService>();

            //JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Helpers.GetConfig("JWT:Audience"),
                    ValidIssuer = Helpers.GetConfig("JWT:Issuer"),
                    ClockSkew = TimeSpan.Zero,// It forces tokens to expire exactly at token expiration time instead of 5 minutes later
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Helpers.GetConfig("JWT:Key")))
                };
            });

            //redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Helpers.GetConfig("Redis:ConnectionString");
                options.InstanceName = Helpers.GetConfig("Redis:RedisTemplate");
            });

            services
                .AddCustomCaching()
                .AddCustomOptions(Configuration)
                .AddCorrelationIdFluent()
                .AddCustomRouting()
                .AddResponseCaching()
                .AddCustomResponseCompression()
                .AddCustomStrictTransportSecurity()
                .AddHttpContextAccessor()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddCustomApiVersioning()
                .AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddDataAnnotations()
                .AddJsonFormatters()
                .AddCustomJsonOptions(HostingEnvironment)
                .AddCustomCors(Configuration)
                .AddCustomMvcOptions(HostingEnvironment)
                .Services
                .AddProjectServices()
                .BuildServiceProvider();
            //ApplicationCollection.AppStartUp(services.BuildServiceProvider());


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Info { Title = "Templates API", Version = "v1" });
                var xmlFile = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
                c.OperationFilter<MyHeaderFilter>();
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
                options.Limits.MaxRequestHeadersTotalSize = 1048576; //this is max value
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Logging
            loggerFactory.AddSerilog();
            // Pass a GUID in a X-Correlation-ID HTTP header to set the HttpContext.TraceIdentifier.
            app.UseCorrelationId()
            .UseForwardedHeaders()
            .UseResponseCaching()
            .UseResponseCompression()
            .UseCors(CorsPolicyName.AllowAny)
            .UseDeveloperErrorPages()
            // .UseIf(
            //     this.hostingEnvironment.IsDevelopment(),
            //     x => x.UseDeveloperErrorPages())
            .UseStaticFilesWithCacheControl()
            .UseAuthentication()
            .UseIf(
                Configuration["Cors:AllowAll"] == "true",
                x => x.UseCors(CorsPolicyName.AllowAny))
            .UseIf(
                Configuration["Cors:AllowAll"] == "true",
                x => x.UseCors(CorsPolicyName.AllowFrontEnd).UseCors(CorsPolicyName.AllowThirdparty))
            .UseMvc();
            app.UseCors("CorsPolicy");


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Templates API V1");
            });
        }
    }
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class MyHeaderFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "X-UserName",
                In = "header",
                Type = "string",
                Required = false,
            });
            //operation.Parameters.Add(new NonBodyParameter
            //{
            //    Name = "X-PermissionToken",
            //    In = "header",
            //    Type = "string",
            //    Required = false
            //});
            //operation.Parameters.Add(new NonBodyParameter
            //{
            //    Name = "X-AccessToken",
            //    In = "header",
            //    Type = "string",
            //    Required = false
            //});
            //operation.Parameters.Add(new NonBodyParameter
            //{
            //    Name = "X-UserId",
            //    In = "header",
            //    Type = "string",
            //    Required = false
            //});
        }
    }
}
