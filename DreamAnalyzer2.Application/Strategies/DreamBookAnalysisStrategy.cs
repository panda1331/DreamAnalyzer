using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DreamAnalyzer2.Application.Strategies
{
    public class DreamBookAnalysisStrategy : IAnalysisStrategy
    {
        private readonly IDreamAiClient _aiClient;
        private readonly string _selectedBook;

        public DreamBookAnalysisStrategy(IDreamAiClient aiClient, string? selectedBook = null)
        {
            _aiClient = aiClient;
            _selectedBook = selectedBook ?? "miller";
        }

        public async Task<AnalysisResponseDto> AnalyzeAsync(string content)
        {
            string authorName = "Миллера";
            string authorStyle = "аналитический, практичный, связывающий бытовые детали с житейскими предзнаменованиями";
            
            switch (_selectedBook.ToLower().Trim())
            {
                case "vanga":
                    authorName = "Ванги";
                    authorStyle = "пророческий, мистический, интуитивный, сфокусированный на судьбе, знаках природы и духовных предупреждениях";
                    break;
                case "nostradamus":
                    authorName = "Нострадамуса";
                    authorStyle = "метафорический, глобальный, связывающий личные сны с масштабными переменами, скрытыми тайнами и предсказаниями на будущее";
                    break;
                default:
                    break;
            }
             
            string systemPrompt =
                $"Ты — легендарный толкователь сновидений. Твоя задача — проанализировать сон, используя исключительно Сонник {authorName}.\n" +
                $"Твой стиль изложения должен быть СТРОГО в духе автора сонника. Твой тон: {authorStyle}.\n\n" +

                "Ты должен сформировать ответ в поле 'interpretation' и СТРОГО разбить его на три логических блока (используй теги <br><br> для переноса строк):\n" +
                $"1. <b>К чему это приснилось по соннику {authorName}:</b> (Объясни скрытый смысл образов и знамений сна в стиле автора).\n" +
                "2. <b>Что вас ждет в будущем:</b> (Опиши грядущие изменения в жизни, делах или эмоциональном состоянии).\n" +
                "3. <b>Какое событие наступит в ближайшее время:</b> (Дай конкретное предзнаменование на ближайшие дни).\n\n" +

                "Правила форматирования ответа:\n" +
                "1. Возвращай ответ СТРОГО в формате JSON в одну строку (без физических знаков Enter).\n" +
                "2. Внутри текста 'interpretation' КАТЕГОРИЧЕСКИ ЗАПРЕЩЕНО использовать стандартные двойные кавычки \". Используй ТОЛЬКО кавычки-ёлочки « » или одинарные кавычки.\n" +
                "3. В поле 'mood' укажи английское название настроения сна: 'Peaceful', 'Anxious', 'NightMare' или 'Lucid'.\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"<b>К чему это приснилось по соннику:</b> ...<br><br> <b>Что вас ждет в будущем:</b> ...<br><br> <b>Какое событие наступит:</b> ...\", \"mood\": \"Peaceful\", \"symbols\": [\"Сонник\"]}";

            string rawJson = await _aiClient.GetJsonCompletionAsync(systemPrompt, content);
            int firstBracket = rawJson.IndexOf('{');
            int lastBracket = rawJson.LastIndexOf('}');
            if (firstBracket == -1 || lastBracket == -1 || lastBracket < firstBracket)
            {
                throw new Exception($"ИИ вернул некорректный формат ответа. Исходный текст: {rawJson}");
            }
            rawJson = rawJson.Substring(firstBracket, lastBracket - firstBracket + 1);

            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;

            return new AnalysisResponseDto
            {
                Title = $"Сонник {authorName}",
                Interpretation = root.GetProperty("interpretation").GetString() ?? "",
                MoodName = root.GetProperty("mood").GetString() ?? "Peaceful",
                Strategy = "dreambook",
                Symbols = new List<string>()
            };
        }
    }
}
