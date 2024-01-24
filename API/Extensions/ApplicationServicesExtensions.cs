
using API.Data;
using API.Helpers;
using API.Intefaces;
using API.services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public  static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt=>{
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCors();
            services.Configure<CloudinarySetting>(config.GetSection("CloudinarySetting"));
            services.AddScoped<IPhotoService ,PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository,MessageRepository>();
            return services;
        }
    }
}