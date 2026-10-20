using DreamAnalyzer.UI.Tests.Pages;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace DreamAnalyzer.UI.Tests.Tests
{
    public class AuthTests : PageTest
    {
        private const string FrontendUrl = "https://localhost:4050";
        private const string BackendUrl = "https://localhost:7021";
        private string _uniqueEmail = string.Empty;
        [SetUp]
        public async Task Setup()
        {
            _uniqueEmail = $"ui_test_{Guid.NewGuid()}@test.com";

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler);

            client.DefaultRequestVersion = HttpVersion.Version11;
            client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

            var response = await client.PostAsJsonAsync($"{BackendUrl}/api/authentication/register", new
            {
                username = "uitester",
                email = _uniqueEmail,
                password = "Test123"
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Не удалось создать пользователя. Status: {response.StatusCode}. Body: {errorText}");
            }
        }

        [Test]
        public async Task SuccessLogin_RedirectsToDreamsPage()
        {
            var loginPage = new LoginPage(Page);
            await loginPage.GoToAsync(FrontendUrl);

            await loginPage.LoginAsync(_uniqueEmail, "Test123");

            await Page.WaitForURLAsync("**/dreams");

            var title = Page.Locator("h2.myDreamsTitle");
            await Expect(title).ToHaveTextAsync("Мои сны");
        }

        [Test]
        public async Task Login_WithWrongPassword_ShowsAlert()
        {
            var loginPage = new LoginPage(Page);
            await loginPage.GoToAsync(FrontendUrl);
            var dialogMessage = "";
            Page.Dialog += async (_, dialog) =>
            {
                dialogMessage = dialog.Message;
                await dialog.AcceptAsync();
            };

            await loginPage.LoginAsync(_uniqueEmail, "WrongPassword_123");

            await Page.WaitForTimeoutAsync(2000);
            Assert.That(dialogMessage, Does.Contain("Wrong password"), "Должно появиться сообщение о неверном пароле");

            Assert.That(Page.Url, Does.Not.Contain("/dreams"));
        }

        [Test]
        public async Task CreateDream_Success_AppearsInList()
        {
            var loginPage = new LoginPage(Page);
            await loginPage.GoToAsync(FrontendUrl);
            await loginPage.LoginAsync(_uniqueEmail, "Test123");
            await Page.WaitForURLAsync("**/dreams");

            var dreamsPage = new DreamsPage(Page);
            string uniqueDreamTitle = $"Сон_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await dreamsPage.ClickAddDreamAsync();
            await dreamsPage.FillDreamAsync(uniqueDreamTitle, "Мне снился огромный замок и дракон.");

            await Page.WaitForURLAsync($"{FrontendUrl}/dreams");

            var dreamCardTitle = Page.Locator($"h3:has-text('{uniqueDreamTitle}')");
            await Expect(dreamCardTitle).ToBeVisibleAsync();
        }

        [Test]
        public async Task SearchDream_FiltersResults()
        {
            var loginPage = new LoginPage(Page);
            await loginPage.GoToAsync(FrontendUrl);
            await loginPage.LoginAsync(_uniqueEmail, "Test123");
            await Page.WaitForURLAsync("**/dreams");

            var searchInput = Page.Locator(".search-input");
            await searchInput.FillAsync("несуществующий_текст_для_поиска");

            var noDreamsText = Page.Locator(".no-dreams"); 
            await Expect(noDreamsText).ToBeVisibleAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var screenshotPath = Path.Combine(TestContext.CurrentContext.TestDirectory, $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:HHmmss}.png");

                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });
                TestContext.Out.WriteLine($"Screenshot saved to: {screenshotPath}");
            }
        }
    }
}
