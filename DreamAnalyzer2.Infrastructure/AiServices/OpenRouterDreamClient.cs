using DreamAnalyzer2.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DreamAnalyzer2.Infrastructure.AiServices
{
    public class OpenRouterDreamClient : IDreamAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public OpenRouterDreamClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            var apiKey = configuration["OpenRouterSettings:ApiKey"]
                ?? throw new ArgumentNullException("Api-key OpenRouter is not found in configuration");
            _model = configuration["OpenRouterSettings:Model"] ?? "meta-llama/llama-3.3-70b-instruct";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "DreamAnalyzer");
        }

        public async Task<string> GetJsonCompletionAsync(string systemPrompt, string userDreamText, CancellationToken cancellationToken = default)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",  content = userDreamText }
                },
            };

            var response = await _httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", requestBody, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken);

            var aiContentJson = jsonResponse?.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return aiContentJson ?? "{}";
        }
    }
}
