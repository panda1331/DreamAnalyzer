using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Infrastructure.AiServices;
using DreamAnalyzer2.Infrastructure.Data;
using DreamAnalyzer2.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DreamAnalyzer2.Tests.Integration
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        public StubHttpMessageHandler AiHandler { get; set; } = new StubHttpMessageHandler(
            """
            {
             "choices": [{
              "message": {
                "content": "{\"interpretation\": \"stub\", \"mood\": \"Peaceful\", \"symbols\": []}"
              }
            }]
            }
            """);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.UseSetting("JwtSettings:Secret", "test-secret-key-at-least-32-characters-long!!");
            builder.UseSetting("JwtSettings:Issuer", "DreamAnalyzerTest");
            builder.UseSetting("JwtSettings:Audience", "DreamAnalyzerTest");
            builder.UseSetting("JwtSettings:ExpiryHours", "1");

            builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=test;Database=test");

            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["OpenRouterSettings:ApiKey"] = "test-api-key",
                    ["OpenRouterSettings:BaseUrl"] = "https://test.api/v1/",
                    ["OpenRouterSettings:Model"] = "test-model"
                });
            });

            builder.ConfigureServices(services =>
            {
                var toRemove = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GetGenericTypeDefinition()
                        == typeof(IDbContextOptionsConfiguration<>))
                ).ToList();

                foreach (var descriptor in toRemove)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                services.RemoveAll<IDreamAiClient>();

                services.AddScoped<IDreamAiClient>(sp =>
                {
                    var cfg = sp.GetRequiredService<IConfiguration>();
                    var http = new HttpClient(AiHandler)
                    {
                        BaseAddress = new Uri("https://test.api/v1/")
                    };
                    return new OpenRouterDreamClient(http, cfg);
                });
            });
        }
    }
}