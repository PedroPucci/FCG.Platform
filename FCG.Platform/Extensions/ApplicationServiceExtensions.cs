using FCG.Platform.Extensions.SwaggerDocumentation;
using FCG.Platform.Infrastracture.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FCG.Platform.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(opt =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API FCG.Platform",
                    Description = @"
                        A **API FCG.Platform** é uma solução moderna para gerenciamento de usuários e bibliotecas de jogos digitais. 
                        Desenvolvida como base da plataforma **FIAP Cloud Games (FCG)**, ela permite o controle eficiente de contas, 
                        autenticação segura e organização dos jogos adquiridos pelos usuários.

                        **Principais Benefícios:**
                        - Cadastro e gerenciamento de usuários com autenticação JWT.
                        - Controle da biblioteca de jogos adquiridos.
                        - Segurança com autenticação e autorização por níveis de acesso.
                        - Estrutura preparada para evolução com novas funcionalidades como matchmaking e servidores online.

                        Com a **FCG.Platform**, a base da plataforma de games educacionais é construída de forma escalável, 
                        segura e pronta para crescimento!
                        ",
                });

                opt.OperationFilter<CustomOperationDescriptions>();
            });

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("WebApiDatabase"));
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:4200");
                });
            });

            //services.AddScoped<IRepositoryUoW, RepositoryUoW>();
            //services.AddScoped<TokenService>();
            //services.AddScoped<BCryptoAlgorithm>();
            //services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();
            //services.AddScoped<AuthService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<TokenService>();
            //services.AddScoped<BCryptoAlgorithm>();


            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            return services;
        }
    }
}