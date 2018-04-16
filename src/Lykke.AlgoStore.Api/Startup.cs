﻿using Autofac;
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
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Repositories;
using Lykke.AlgoStore.Core.Services;
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

                services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(PermissionFilter));
                });

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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime, IUserPermissionsService permissionsService, IUserRolesService rolesService, IRolePermissionMatchRepository rolePermissionMatchRepository)
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

                appLifetime.ApplicationStarted.Register(() => StartApplication(permissionsService, rolesService, rolePermissionMatchRepository).Wait());
                appLifetime.ApplicationStopped.Register(() => CleanUp().Wait());
            }
            catch (Exception ex)
            {
                Log?.WriteFatalErrorAsync(nameof(Startup), nameof(ConfigureServices), "", ex).Wait();
                throw;
            }
        }

        private async Task StartApplication(IUserPermissionsService permissionsService, IUserRolesService rolesService, IRolePermissionMatchRepository rolePermissionMatchRepository)
        {
            try
            {
                await SeedPermissions(permissionsService, rolesService, rolePermissionMatchRepository);
                await SeedRoles(rolesService, permissionsService, rolePermissionMatchRepository);
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

        private async Task SeedPermissions(IUserPermissionsService permissionsService, IUserRolesService rolesService, IRolePermissionMatchRepository rolePermissionMatchRepository)
        {
            await Log.WriteInfoAsync("", "", "Permission seed started");

            // Extract controller methods
            Permissions = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && t.ReflectedType == null && String.Equals(t.Namespace, "Lykke.AlgoStore.Api.Controllers", StringComparison.Ordinal))
            .SelectMany(c => c.GetMethods().Where(m => m.ReturnType == typeof(Task<IActionResult>) && m.GetCustomAttribute(typeof(RequirePermissionAttribute)) != null))
            .Select(i => new UserPermissionData()
                {
                    Id = i.Name,
                    Name = i.ReflectedType.Name,
                    DisplayName = Regex.Replace(i.Name, "([A-Z]{1,2}|[0-9]+)", " $1").TrimStart()
            })
            .ToList();

            // check if we should delete any old permissions
            var allPermissionIds = await permissionsService.GetAllPermissionsAsync();
            
            // TODO find a way to optimize this
            var permissionsIdsForDeletion = allPermissionIds.Select(p => p.Id).Where(perm => !Permissions.Select(perms => perms.Id).Contains(perm)).ToList(); 
            

            if (permissionsIdsForDeletion.Count > 0)
            {
                var allRoles = await rolesService.GetAllRolesAsync();

                // delete old unneeded permissions
                foreach (var permissionId in permissionsIdsForDeletion)
                {
                    // first check if the permission has been referenced in any role
                    var matches = allRoles.Where(role => role.Permissions.Select(p => p.Id).Contains(permissionId)).ToList();

                    // if the permission is referenced, remove the reference
                    if(matches.Count > 0)
                    {
                        foreach (var reference in matches)
                        {
                            await rolePermissionMatchRepository.RevokePermission(new RolePermissionMatchData()
                            {
                                RoleId = reference.Id,
                                PermissionId = permissionId
                            });
                        }
                    }

                    // finally delete the permission
                    await permissionsService.DeletePermissionAsync(permissionId);
                }
            }

            // refresh current permissions
            foreach (var permission in Permissions)
            {
                await permissionsService.SavePermissionAsync(permission);
            }            
        }

        private async Task SeedRoles(IUserRolesService rolesService, IUserPermissionsService permissionsService, IRolePermissionMatchRepository rolePermissionMatchRepository)
        {
            await Log.WriteInfoAsync("", "", "Role seed started");

            var allRoles = await rolesService.GetAllRolesAsync();

            // Check if admin role exists, if not - seed it
            // Note: Only the original admin role cannot be deleted
            var adminRole = allRoles.Where(role => role.Name == "Admin" && !role.CanBeDeleted).FirstOrDefault();

            // If there is no admin role, we need to seed it
            if (adminRole == null)
            {
                adminRole = new UserRoleData()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    CanBeDeleted = false,
                    CanBeModified = false
                };

                // Create the Admin role
                await rolesService.SaveRoleAsync(adminRole);
            }

            // Check if user role exists, if not - seed it. Don't touch it if it exists
            // Note: Only the original user role cannot be deleted
            var userRole = allRoles.Where(role => role.Name == "User" && !role.CanBeDeleted).FirstOrDefault();

            if(userRole == null)
            {
                userRole = new UserRoleData()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    CanBeDeleted = false,
                    CanBeModified = true
                };

                // Create the User role
                await rolesService.SaveRoleAsync(userRole);
            }

            // Seed the permissions for the admin role
            foreach (var permission in Permissions)
            {
                var match = new RolePermissionMatchData()
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                };

                await rolePermissionMatchRepository.AssignPermissionToRoleAsync(match);
            }
        }
    }
}
