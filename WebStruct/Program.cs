using Asp.Versioning;
using CompModels.ModelsAlghoritms.Handler;
using HealthChecks.UI.Client;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Text;
using UsersRoles.DAL.CodeFirst;
using UsersRoles.Repositories.Interfaces;
using UsersRoles.Repositories.Repositories;
using UsersRoles.Services.Enums;
using UsersRoles.Services.Implementations;
using UsersRoles.Services.Interfaces;
using WebStruct.HealthChecks;
using WebStruct.HostedServices;
using WebStruct.JWT;
using WebStruct.MySwagger;
using WebStruct.RateLimit;
using WebStruct.Shared;

namespace WebStruct
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("jwt.json", optional: false, reloadOnChange: false);
            builder.Configuration.AddJsonFile("connectionStrings.json", optional: false, reloadOnChange: false);

            // Конфигурация JWT
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            builder.Services.Configure<DbConnectionOptions>(builder.Configuration.GetSection("ConnectionStrings"));

            // RATE-LIMIT
            builder.Services.AddSingleton<IJwtService, JwtService>();
            builder.Services.AddSingleton<IRateLimitService, RateLimitService>();

            // JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("WebStructOnly", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            //ADD SWAGGER
            builder.Services.AddSwaggerGen(swagger =>
            {
                //SWAGGER DEFAULTS CONFIGS
                swagger.OperationFilter<SwaggerDefaults>();

                swagger.SupportNonNullableReferenceTypes();

                //SWAGGER-BEARRER:
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT-token in format: \"Bearer myTokenFullValue\"",
                });
                swagger.UseAllOfToExtendReferenceSchemas();
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                //REGISTER ALL ASSEMBLIES COMMENTS TO SWAGGER
                var myAppAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName().Name).ToList();
                var xmlFilesPath = myAppAssemblies.Select(x => $"{x}.xml").ToList();
                foreach (var path in xmlFilesPath)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, path);
                    if (File.Exists(xmlPath))
                    {
                        swagger.IncludeXmlComments(xmlPath);
                    }
                }
            });

            //API VERSION
            builder.Services.AddApiVersioning(api =>
            {
                api.DefaultApiVersion = new ApiVersion(1.0);
                api.ReportApiVersions = true;
            })
            .AddApiExplorer(api =>
            {
                api.GroupNameFormat = "'v'VVV";
                api.AssumeDefaultVersionWhenUnspecified = true;
                api.SubstituteApiVersionInUrl = true;
            });

            //SWAGGER OPTIONS
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, MySwaggerOptions>();

            builder.Services.AddHealthChecks()
                            .AddCheck<CalculationsStatusesHealthCheck>("Выполнение запрошенных пользователями расчётов");

            //WEBSTRUCTOCNTEXT = USERS-ROLES-PERMISSIONS.
            builder.Services.AddDbContext<WebStructContext>
                (options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ASP NET IDENTITY USERS-ROLES
            builder.Services.AddIdentity<WebStructUser, WebStructRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 12;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<WebStructContext>()
            .AddDefaultTokenProviders();

            // USERS-ROLES ENDPOINTS AUTHORIZATION
            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in Enum.GetValues<PermissionEnum>())
                {
                    options.AddPolicy(permission.GetName(),
                        policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddTransient<IPermissionsRepository, PermissionsRepository>();
            builder.Services.AddTransient<IUsersRepository, UsersRepository>();
            builder.Services.AddTransient<IRolesRepository, RolesRepository>();
            builder.Services.AddTransient<IPermissionsService, PermissionsService>();
            builder.Services.AddTransient<IUsersService, UsersService>();
            builder.Services.AddTransient<IRolesService, RolesService>();

            builder.Services.AddTransient<ICalculateRequestsHandler, CalculateRequestsHandler>();

            builder.Services.AddHostedService<CalculationsHandleWorker>();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            // Rate Limiting Middleware
            app.UseMiddleware<RateLimitMiddleware>();

            // JWT Validation Middleware
            app.UseMiddleware<JwtValidationMiddleware>();

            app.UseCors("WebStructOnly");

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(swagger =>
            {
                var specifications = app.DescribeApiVersions();
                foreach (var spec in specifications)
                {
                    var subpath = $"/swagger/{spec.GroupName}/swagger.json";
                    var title = spec.GroupName.ToUpperInvariant();
                    swagger.SwaggerEndpoint(subpath, title);
                }
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}
