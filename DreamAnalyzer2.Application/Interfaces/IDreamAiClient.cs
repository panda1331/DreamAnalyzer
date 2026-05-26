using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces
{
    public interface IDreamAiClient
    {
        Task<string> GetJsonCompletionAsync(string systemPrompt, string userDreamText, CancellationToken cancellationToken = default);
    }
}
