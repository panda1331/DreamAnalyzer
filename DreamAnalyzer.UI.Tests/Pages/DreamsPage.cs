using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer.UI.Tests.Pages
{
    public class DreamsPage
    {
        private readonly IPage _page;

        public DreamsPage(IPage page)
        {
            _page = page;
        }

        public ILocator PageTitle => _page.Locator("h2.myDreamsTitle");
        public ILocator AddDreamBtn => _page.Locator(".dreamTitleContainerCreateBtn");

        public ILocator DreamTitleInput => _page.Locator("input[type='text']");
        public ILocator DreamContentInput => _page.Locator("textarea");
        public ILocator SaveBtn => _page.Locator("button[type='submit']");

        public async Task ClickAddDreamAsync()
        {
            await AddDreamBtn.ClickAsync();
            await _page.WaitForSelectorAsync("h2:has-text('Опиши свой сон:')");
        }

        public async Task FillDreamAsync(string title, string content)
        {
            await DreamTitleInput.FillAsync(title);
            await DreamContentInput.FillAsync(content);
            await SaveBtn.ClickAsync();
        }
    }
}
