using DreamAnalyzer2.Infrastructure.AiServices;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DreamAnalyzer2.Tests.Integration
{
    public class OpenRouterDreamClientsTests
    {
        private static IConfiguration BuildConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenRouterSettings:ApiKey"] = "test-key",
                ["OpenRouterSettings:Model"] = "test-model",
                ["OpenRouterSettings:BaseUrl"] = "https://fake.api/v1/"
            })
            .Build();

        [Fact]
        public async Task GetJsonCompletionAsync_WithValidResponse_ReturnsContent()
        {
            var body = """
                { "choices": [{ "message": { "content": "{\"interpretation\":\"t\"}" } }] }
            """;
            var handler = new StubHttpMessageHandler(body);
            var http = new HttpClient(handler);
            var client = new OpenRouterDreamClient(http, BuildConfig());

            var result = await client.GetJsonCompletionAsync("prompt", "dream");

            result.Should().Contain("interpretation");
            handler.Requests.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public async Task GetJsonCompletionAsync_WhenServerReturnsError_Throws(HttpStatusCode code)
        {
            var handler = new StubHttpMessageHandler("error", code);
            var http = new HttpClient(handler);
            var client = new OpenRouterDreamClient(http, BuildConfig());

            Func<Task> act = () => client.GetJsonCompletionAsync("prompt", "text");

            await act.Should().ThrowAsync<HttpRequestException>();
        }
    }
}
