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
    public class DreamScenarioTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public DreamScenarioTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> RegisterAndGetTokenAsync()
        {
            var email = $"user_{Guid.NewGuid()}@test.com";
            var resp = await _client.PostAsJsonAsync(
                "/api/authentication/register",
                new RegisterDto
                {
                    Username = "testuser",
                    Email = email,
                    Password = "Test123"
                });

            var data = await resp.Content
                .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            return data!.Data.Token;
        }

        [Fact]
        public async Task DreamLifecycle_Create_Analyze_Delete_FullCycle()
        {
            var token = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var createDto = new CreateDreamDto
            {
                Title = "Коридор зеркал",
                Content = "Мне снилась кошка",
                DreamDate = DateTime.UtcNow
            };

            var createResp = await _client.PostAsJsonAsync("/api/Dreams", createDto);
            createResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var createData = await createResp.Content
                .ReadFromJsonAsync<ApiResponse<DreamResponseDto>>();
            var dreamId = createData!.Data.Id;
            dreamId.Should().NotBeEmpty();

            var analyzeResp = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=symbols", null);
            analyzeResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var analysisData = await analyzeResp.Content
                .ReadFromJsonAsync<ApiResponse<AnalysisResponseDto>>();
            analysisData!.Data.Strategy.Should().Be("symbols");

            var deleteResp = await _client.DeleteAsync($"/api/Dreams/{dreamId}");
            deleteResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResp = await _client.GetAsync($"/api/Dreams/{dreamId}");

            getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DreamLifecycle_EditBeforeAnalyze_ThenAnalyze_Succeeds()
        {
            var token = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/Dreams",
                new CreateDreamDto
                {
                    Title = "Море",
                    Content = "Изначальный текст",
                    DreamDate = DateTime.UtcNow
                });

            var dreamId = (await createResp.Content
                .ReadFromJsonAsync<ApiResponse<DreamResponseDto>>())!.Data.Id;

            var updateDto = new UpdateDreamDto
            {
                Title = "Море (исправлено)",
                Content = "Изменённый текст",
                DreamDate = DateTime.UtcNow
            };

            var updateResp = await _client.PutAsJsonAsync(
                $"/api/Dreams/{dreamId}", updateDto);
            updateResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var analyzeResp = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=symbols", null);
            analyzeResp.StatusCode.Should().Be(HttpStatusCode.OK);

            await _client.DeleteAsync($"/api/Dreams/{dreamId}");
        }

        [Fact]
        public async Task DreamLifecycle_ReAnalyze_UpdatesExistingAnalysis()
        {
            var token = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var createResp = await _client.PostAsJsonAsync("/api/Dreams",
                new CreateDreamDto
                {
                    Title = "Test",
                    Content = "Кошка",
                    DreamDate = DateTime.UtcNow
                });

            var dreamId = (await createResp.Content
                .ReadFromJsonAsync<ApiResponse<DreamResponseDto>>())!.Data.Id;

            var firstAnalyze = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=symbols", null);
            firstAnalyze.StatusCode.Should().Be(HttpStatusCode.OK);

            var secondAnalyze = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=symbols", null);

            secondAnalyze.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DreamLifecycle_AnalyzeAnothersDream_ReturnsError()
        {
            var token1 = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token1);

            var createResp = await _client.PostAsJsonAsync("/api/Dreams",
                new CreateDreamDto
                {
                    Title = "Test",
                    Content = "Content",
                    DreamDate = DateTime.UtcNow
                });

            var dreamId = (await createResp.Content
                .ReadFromJsonAsync<ApiResponse<DreamResponseDto>>())!.Data.Id;

            var token2 = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token2);

            var resp = await _client.PostAsync(
                $"/api/Dreams/{dreamId}/analyze?strategyType=symbols", null);

            resp.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.Forbidden);
        }
    }
}
