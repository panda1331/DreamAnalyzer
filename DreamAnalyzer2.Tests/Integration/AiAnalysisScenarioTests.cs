using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
using DreamAnalyzer2.Application.DTOs.Requests.User;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Shared.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DreamAnalyzer2.Tests.Integration
{
    public class AiAnalysisScenarioTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AiAnalysisScenarioTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Analyze_WithJungianStrategy_CallsExternalAiThroughStub()
        {
            _factory.AiHandler = new StubHttpMessageHandler("""
            {
              "choices": [{
                "message": {
                  "content": "{\"interpretation\": \"Архетип Тени\", \"mood\": \"Peaceful\", \"symbols\": [\"море\"]}"
                }
              }]
            }
            """);

            var email = $"user_{Guid.NewGuid()}@test.com";
            var reg = await _client.PostAsJsonAsync("/api/authentication/register",
                new RegisterDto { Username = "testuser", Email = email, Password = "Test123" });

            var regBody = await reg.Content.ReadAsStringAsync();
            reg.StatusCode.Should().Be(HttpStatusCode.OK, because: $"register body was: {regBody}");

            var token = (await reg.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>())!.Data.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/Dreams", new CreateDreamDto
            {
                Title = "Сон",
                Content = "Мне снилось море",
                DreamDate = DateTime.UtcNow
            });
            var dreamId = (await createResp.Content
                .ReadFromJsonAsync<ApiResponse<DreamResponseDto>>())!.Data.Id;

            var analyzeResp = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=jungian", null);

            analyzeResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await analyzeResp.Content
                .ReadFromJsonAsync<ApiResponse<AnalysisResponseDto>>();
            data!.Data.Interpretation.Should().Be("Архетип Тени");
            data.Data.Strategy.Should().Be("jungian");

            _factory.AiHandler.Requests.Should().HaveCount(1);
            _factory.AiHandler.Requests[0].RequestUri!.Host.Should().Be("test.api");

            await _client.DeleteAsync($"/api/Dreams/{dreamId}");
        }

        [Fact]
        public async Task Analyze_WhenExternalAiReturns500_ReturnsServerError()
        {
            _factory.AiHandler = new StubHttpMessageHandler(
                "error", HttpStatusCode.InternalServerError);

            var email = $"user_{Guid.NewGuid()}@test.com";
            var reg = await _client.PostAsJsonAsync("/api/authentication/register",
                new RegisterDto { Username = "testuser", Email = email, Password = "Test123" });

            var regBody = await reg.Content.ReadAsStringAsync();
            reg.StatusCode.Should().Be(HttpStatusCode.OK, because: $"register body was: {regBody}");

            var token = (await reg.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>())!.Data.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/Dreams", new CreateDreamDto
            {
                Title = "Сон",
                Content = "text",
                DreamDate = DateTime.UtcNow
            });
            var dreamId = (await createResp.Content
                .ReadFromJsonAsync<ApiResponse<DreamResponseDto>>())!.Data.Id;

            var resp = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=jungian", null);

            resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
