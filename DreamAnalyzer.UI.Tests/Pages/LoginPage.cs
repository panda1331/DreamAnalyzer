using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer.UI.Tests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page)
        {
            _page = page;
        }

        private ILocator EmailInput => _page.Locator("input[type='text']");
        private ILocator PasswordInput => _page.Locator("input[type='password']");
        private ILocator SubmitButton => _page.Locator("button[type='submit']");

        public async Task GoToAsync(string baseUrl)
        {
            await _page.GotoAsync($"{baseUrl}/login");

            await _page.WaitForSelectorAsync("input[type='text']", new PageWaitForSelectorOptions { Timeout = 10000 });
        }

        public async Task LoginAsync(string email, string password)
        {
            await EmailInput.FillAsync(email);
            await PasswordInput.FillAsync(password);
            await SubmitButton.ClickAsync();
        }
    }
}
