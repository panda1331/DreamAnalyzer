using DreamAnalyzer2.Application.DTOs.Requests.User;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Shared.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace DreamAnalyzer2.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_Then_Login_FullFlow_Works()
        {
            var email = $"user_{Guid.NewGuid()}@test.com";
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = email,
                Password = "Test123"
            };

            var registerResp = await _client.PostAsJsonAsync(
                "/api/authentication/register", registerDto);

            registerResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var registerData = await registerResp.Content
                .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            registerData.Should().NotBeNull();
            registerData!.Success.Should().BeTrue();
            registerData.Data.Token.Should().NotBeNullOrEmpty();

            var loginDto = new LoginDto { Email = email, Password = "Test123" };
            var loginResp = await _client.PostAsJsonAsync(
                "/api/authentication/login", loginDto);

            loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginData = await loginResp.Content
                .ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            loginData!.Data.Token.Should().NotBeNullOrEmpty();
            loginData.Data.Email.Should().Be(email);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsUnauthorized()
        {
            var email = $"dup_{Guid.NewGuid()}@test.com";
            var dto = new RegisterDto
            {
                Username = "testuser",
                Email = email,
                Password = "Test123"
            };

            await _client.PostAsJsonAsync("/api/authentication/register", dto);

            var secondResp = await _client.PostAsJsonAsync(
                "/api/authentication/register", dto);

            secondResp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("wrong@test.com", "Test123")]
        [InlineData("valid@test.com", "WrongPass")]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized(
            string email, string password)
        {
            var loginDto = new LoginDto { Email = email, Password = password };

            var response = await _client.PostAsJsonAsync(
                "/api/authentication/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetProtectedResource_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/Dreams");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
