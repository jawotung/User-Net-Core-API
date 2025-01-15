//using Application.Interfaces;
//using Application.Services;
//using Infrastructure.Repositories;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
//using WebAPI;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<POSDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("POSDBContext"));
            });

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepositories, UserRepositories>();

            return services;
        }
    }
}
