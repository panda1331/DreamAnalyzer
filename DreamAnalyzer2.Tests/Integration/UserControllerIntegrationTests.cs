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
    public class UserControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UserControllerIntegrationTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> RegisterAndGetTokenAsync()
        {
            var email = $"user_{Guid.NewGuid()}@test.com";
            var resp = await _client.PostAsJsonAsync(
                "/api/authentication/register",
                new RegisterDto { Username = "testuser", Email = email, Password = "Test123" });

            var data = await resp.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            return data!.Data.Token;
        }

        [Fact]
        public async Task GetProfileInfo_WithValidToken_ReturnsProfile()
        {
            var token = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.GetAsync("/api/users");
            var data = await resp.Content.ReadFromJsonAsync<ApiResponse<ProfileResponseDto>>();

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            data!.Success.Should().BeTrue();
            data.Data.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetStatistics_WithValidToken_ReturnsStatistics()
        {
            var token = await RegisterAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.GetAsync("/api/users/statistics");
            var data = await resp.Content.ReadFromJsonAsync<ApiResponse<StatisticsResponseDto>>();

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            data!.Success.Should().BeTrue();
            data.Data.TotalDreams.Should().Be(0);
        }

        [Fact]
        public async Task GetProfileInfo_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/users");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
