using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Security;
using DreamAnalyzer2.Application.Strategies;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Infrastructure.AiServices;
using DreamAnalyzer2.Infrastructure.Data;
using DreamAnalyzer2.Infrastructure.Repositories;
using DreamAnalyzer2.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDreamRepository, DreamRepository>();
            services.AddScoped<ISymbolRepository, SymbolRepository>();
            services.AddScoped<IAnalysisRepository, AnalysisRepository>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IStrategyFactory, StrategyFactory>();
            services.AddScoped<SymbolStrategy>();

            services.AddHttpClient<IDreamAiClient, OpenRouterDreamClient>(client =>
            {
                var baseUrl = configuration["OpenRouterSettings:BaseUrl"] ?? "https://openrouter.ai/api/v1/";
                client.BaseAddress = new Uri(baseUrl);
            });
            services.AddScoped<SymbolStrategy>();
            services.AddScoped<FreudianAnalysisStrategy>();
            services.AddScoped<JungianAnalysisStrategy>();
            services.AddScoped<CognitiveAnalysisStrategy>();
            services.AddScoped<DreamBookAnalysisStrategy>();

            return services;
        } 
    }
}
