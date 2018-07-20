using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Api.Infrastructure;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Authentication;
using Lykke.AlgoStore.Api.Infrastructure.ContentFilters;
using Lykke.AlgoStore.Api.Infrastructure.Managers;
using Lykke.AlgoStore.Api.Infrastructure.OperationFilters;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.Service.Security.Client.AutorestClient.Models;
using Lykke.AlgoStore.Api.RealTimeStreaming;
using Lykke.AlgoStore.Api.RealTimeStreaming.DataStreamers.WebSockets.Middleware;

namespace Lykke.AlgoStore.Api
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }
        public ILog Log { get; private set; }
        public List<UserPermissionData> Permissions { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfiles(typeof(AutoMapperProfile));
                cfg.AddProfiles(typeof(AutoMapperModelProfile));
            });

            Mapper.AssertConfigurationIsValid();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });

                services.AddMvc(options => { options.Filters.Add(typeof(PermissionFilter)); });

                services.AddScoped<ValidateMimeMultipartContentFilter>();

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration(AlgoStoreConstants.ApiVersion, AlgoStoreConstants.AppName);
                    options.OperationFilter<ApiKeyHeaderOperationFilter>();
                    options.OperationFilter<FileUploadOperationFilter>();
                });

                services.AddLykkeAuthentication();

                var appSettings = Configuration.LoadSettings<AppSettings>();
                Log = LogManager.CreateLogWithSlack(services, appSettings);

                ApplicationContainer = ContainerManager.RegisterAlgoApiModules(services, appSettings, Log);

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                Log?.WriteFatalErrorAsync(nameof(Startup), nameof(ConfigureServices), "", ex).Wait();
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime,
            ISecurityClient securityClient)
        {
            try
            {
                app.UseCors(builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
                app.Use(next => context =>
                {
                    context.Request.EnableRewind();

                    return next(context);
                });

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseMiddleware<AlgoStoreErrorHandlerMiddleware>(AlgoStoreConstants.AppName);

                app.UseAuthentication();

                app.UseMvc();

                app.UseSwagger();
                app.UseSwaggerUI(x =>
                {
                    x.RoutePrefix = "swagger/ui";
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
                app.UseStaticFiles();

                RegisterWebSocketsForRealTimeData(app);

                appLifetime.ApplicationStarted.Register(() => StartApplication(securityClient).Wait());
                appLifetime.ApplicationStopped.Register(() => CleanUp().Wait());
            }
            catch (Exception ex)
            {
                Log?.WriteFatalErrorAsync(nameof(Startup), nameof(ConfigureServices), "", ex).Wait();
                throw;
            }
        }

        private void RegisterWebSocketsForRealTimeData(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(Constants.WebSocketKeepAliveIntervalSeconds),
                ReceiveBufferSize = Constants.WebSocketRecieveBufferSize

            };

            app.UseWebSockets(webSocketOptions);

            app.Map("/live/dummy", (_app) => _app.UseMiddleware<DummyWebSocketsMiddleware>());

            //app.Map("/live/candles", (_app) => _app.UseMiddleware<CandlesWebSocketsMiddleware>());
            //app.Map("/live/trades", (_app) => _app.UseMiddleware<TradesWebSocketsMiddleware>());
            //app.Map("/live/functions", (_app) => _app.UseMiddleware<FunctionsWebSocketsMiddleware>());
        }

        private async Task StartApplication(ISecurityClient securityClient)
        {
            try
            {
                await SeedPermissions(securityClient);

                await SeedRoles(securityClient);

                await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Started");
            }
            catch (Exception ex)
            {
                await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StartApplication), "", ex);
                throw;
            }
        }

        private async Task CleanUp()
        {
            try
            {
                if (Log != null)
                {
                    await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Terminating");
                }

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(CleanUp), "", ex);
                    (Log as IDisposable)?.Dispose();
                }

                throw;
            }
        }

        private async Task SeedPermissions(ISecurityClient securityClient)
        {
            await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, nameof(SeedPermissions), "Permission seed started");

            ExtractPermissionsFromControllers();

            await securityClient.SeedPermissions(Permissions);

            await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, nameof(SeedPermissions), "Permission seed finished");
        }

        private void ExtractPermissionsFromControllers()
        {
            // Extract controller methods
            Permissions = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.ReflectedType == null && t.Namespace == "Lykke.AlgoStore.Api.Controllers")
                .SelectMany(c => c.GetMethods().Where(m =>
                    m.ReturnType == typeof(Task<IActionResult>) &&
                    (m.GetCustomAttribute(typeof(RequirePermissionAttribute)) != null ||
                     m.DeclaringType.GetCustomAttribute(typeof(RequirePermissionAttribute)) != null)))
                .Select(i => new UserPermissionData
                    {
                        Id = i.Name,
                        Name = i.ReflectedType.Name,
                        DisplayName = Regex.Replace(i.Name, "([A-Z]{1,2}|[0-9]+)", " $1").TrimStart(),
                        Description = (i.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description
                })
                .ToList();
        }

        private async Task SeedRoles(ISecurityClient securityClient)
        {
            await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, nameof(SeedRoles), "Role seed started");

            await securityClient.SeedRoles(Permissions);

            await Log.WriteInfoAsync(AlgoStoreConstants.ProcessName, nameof(SeedRoles), "Role seed finished");
        }
    }
}
