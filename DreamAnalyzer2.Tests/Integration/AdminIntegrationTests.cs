using DreamAnalyzer2.Application.DTOs.Requests.User;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Domain.Enums;
using DreamAnalyzer2.Infrastructure.Data;
using DreamAnalyzer2.Shared.Responses;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DreamAnalyzer2.Tests.Integration
{
    public class AdminIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;

        public AdminIntegrationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<(string token, Guid userId)> RegisterAdminAsync()
        {
            var email = $"admin_{Guid.NewGuid()}@test.com";
            var password = "Test123";

            var regResp = await _client.PostAsJsonAsync(
                "/api/authentication/register",
                new RegisterDto { Username = "admin", Email = email, Password = password });

            var regData = await regResp.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            var userId = regData!.Data.Id;

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await db.Users.FirstAsync(u => u.Id == userId);
            user.SetRole(RoleType.Admin);
            await db.SaveChangesAsync();

            var loginResp = await _client.PostAsJsonAsync(
                "/api/authentication/login",
                new LoginDto { Email = email, Password = password });

            var loginData = await loginResp.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            return (loginData!.Data.Token, userId);
        }

        [Fact]
        public async Task GetAllUsers_AsAdmin_ReturnsOkWithUsers()
        {
            var (token, _) = await RegisterAdminAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.GetAsync("/api/admin/users");

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await resp.Content.ReadFromJsonAsync<ApiResponse<List<UserResponseDto>>>();
            data!.Success.Should().BeTrue();
            data.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllDreams_AsAdmin_ReturnsOk()
        {
            var (token, _) = await RegisterAdminAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.GetAsync("/api/admin/dreams");

            resp.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await resp.Content.ReadFromJsonAsync<ApiResponse<List<DreamResponseDto>>>();
            data!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllUsers_WithoutAdminRole_ReturnsForbidden()
        {
            var email = $"user_{Guid.NewGuid()}@test.com";
            var resp = await _client.PostAsJsonAsync(
                "/api/authentication/register",
                new RegisterDto { Username = "user", Email = email, Password = "Test123" });

            var data = await resp.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", data!.Data.Token);

            var response = await _client.GetAsync("/api/admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAllUsers_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/admin/users");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllDreams_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/admin/dreams");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
