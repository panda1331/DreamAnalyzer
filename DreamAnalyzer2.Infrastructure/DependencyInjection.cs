using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Security;
using DreamAnalyzer2.Application.Strategies;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Infrastructure.Data;
using DreamAnalyzer2.Infrastructure.Repositories;
using DreamAnalyzer2.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDreamRepository, DreamRepository>();
            services.AddScoped<ISymbolRepository, SymbolRepository>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IStrategyFactory, StrategyFactory>();
            services.AddScoped<SymbolStrategy>();
            //add strategies

            return services;
        } 
    }
}
