using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using multiTenantManagement.Data.Contexts;
using multiTenantManagement.Models.Identities;
using multiTenantManagement.Services.Contract;
using multiTenantManagement.Services.Implementation;
using multiTenantManagement.Utilities;
using System.Text;

namespace multiTenantManagement.Extensions
{
    public static class ServiceExtensions
    {

        public static void RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            services.ConfigureIdentity();
            services.ConfigureJWT(config);
            services.ConfigureSwagger();
            services.AddServices();
            services.ConfigureContext(config);
            //services.AddAuthorization();

        }
        public static void  ConfigureContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddDbContext<TenantDbContext>(x => x.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                // TODO: Fix the Docker error on this
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // c.IncludeXmlComments(xmlPath);
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(x =>
            {
                x.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

       
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentTenantService, CurrentTenantService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddOptions<JwtSettings>().BindConfiguration("jwtSettings");

            //services.AddTransient<TenantResolver>();
        
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("jwtSettings");
            var secretKey = jwtSettings.GetSection("secret").Value;
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(b =>
            {
                b.SaveToken = true;
                b.RequireHttpsMetadata = false;
                b.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

            });
        }
    }
}
