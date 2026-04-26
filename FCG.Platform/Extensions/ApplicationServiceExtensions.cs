using FCG.Platform.Application.Services;
using FCG.Platform.Application.UnitOfWork;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Extensions.SwaggerDocumentation;
using FCG.Platform.Infrastracture.Connections;
using FCG.Platform.Infrastracture.Repository;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace FCG.Platform.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            var jwtSettings = config.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var secretKey = jwtSettings["SecretKey"];

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("WebApiDatabase"));
            });

            services.AddSwaggerGen(opt =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                opt.CustomSchemaIds(t => t.FullName);
                opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API FCG.Platform",
                    Description = @"
                        A API FCG.Platform é uma solução moderna para gerenciamento de usuários e bibliotecas de jogos digitais. 
                        Desenvolvida como base da plataforma **FIAP Cloud Games (FCG)**, ela permite o controle eficiente de contas, 
                        autenticação segura e organização dos jogos adquiridos pelos usuários.

                        Principais Benefícios:
                        - Cadastro e gerenciamento de usuários com autenticação JWT.
                        - Controle da biblioteca de jogos adquiridos.
                        - Segurança com autenticação e autorização por níveis de acesso.
                        - Estrutura preparada para evolução com novas funcionalidades como matchmaking e servidores online.

                        Com a FCG.Platform, a base da plataforma de games educacionais é construída de forma escalável, segura e pronta para crescimento!
                    "
                });

                opt.OperationFilter<CustomOperationDescriptions>();

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Digite: Bearer {seu_token}"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });

                if (File.Exists(xmlPath))
                {
                    opt.IncludeXmlComments(xmlPath);
                }
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:4200");
                });
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });

            services.AddAuthorization();

            services.AddScoped<IRepositoryUoW, RepositoryUoW>();
            services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IUserService, UserService>();

            services.AddIdentity<UserEntity, ProfileEntity>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            return services;
        }
    }
}