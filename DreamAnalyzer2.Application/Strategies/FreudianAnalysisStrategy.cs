using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DreamAnalyzer2.Application.Strategies
{
    public class FreudianAnalysisStrategy : IAnalysisStrategy
    {
        private readonly IDreamAiClient _aiClient;

        public FreudianAnalysisStrategy(IDreamAiClient aiClient)
        {
            _aiClient = aiClient;
        }

        public async Task<AnalysisResponseDto> AnalyzeAsync(string content)
        {
            string systemPrompt =
                "Ты — Зигмунд Фрейд, выдающийся психоаналитик. Твоя задача — провести глубокий, развернутый и " +
                "детальный психоанализ сновидения пользователя, используя каноны классического психоанализа. " +
                "Анализ должен быть подробным (минимум 3-4 объемных абзаца) и не должен ограничиваться общими фразами.\n\n" +

                "В тексте анализа (поле 'interpretation') ты ОБЯЗАН отразить следующие аспекты:\n" +
                "1. Явное и скрытое содержание сна (Manifest vs. Latent content). Что скрывается за внешними образами?\n" +
                "2. Проявления триады личности: как в образах сна конфликтуют Оно (Id - подавленные влечения), Я (Ego - рациональное) и Сверх-Я (Superego - мораль и цензура).\n" +
                "3. Подавленные желания, страхи и детские комплексы, которые подсознание пытается выразить.\n" +
                "4. Работа механизмов психической защиты (сгущение, смещение, символизация), которые исказили реальный смысл сна.\n\n" +

                "Правила форматирования ответа:\n" +
                "1. Ты обязан вернуть ответ СТРОГО в формате JSON в одну строку (БЕЗ физических переносов строки через Enter).\n" +
                "2. Внутри текста 'interpretation' тебе КАТЕГОРИЧЕСКИ ЗАПРЕЩЕНО использовать стандартные двойные кавычки \". Для выделения цитат или понятий используй ТОЛЬКО кавычки-ёлочки « » или одинарные кавычки ' '.\n" +
                "3. Для разделения аналитического разбора на абзацы внутри поля 'interpretation' ОБЯЗАТЕЛЬНО используй теги <br><br>.\n" +
                "4. Используй английские названия Mood: 'Peaceful', 'Anxious', 'NightMare', 'Lucid'.\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"Первый абзац анализа с понятиями «Оно» и «Я»...<br><br>Второй абзац...\", \"mood\": \"Anxious\", \"symbols\": [\"Волк\", \"Лес\"]}";

            string rawJson = await _aiClient.GetJsonCompletionAsync(systemPrompt, content);
            int firstBracket = rawJson.IndexOf('{');
            int lastBracket = rawJson.LastIndexOf('}');
            if (firstBracket == -1 || lastBracket == -1 || lastBracket < firstBracket)
            {
                throw new Exception($"ИИ вернул некорректный формат ответа. Исходный текст: {rawJson}");
            }
            rawJson = rawJson.Substring(firstBracket, lastBracket - firstBracket + 1);

            // Настраиваем опции для чтения JSON
            var parseOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true // Разрешаем лишние запятые, если ИИ их оставит
            };

            using var doc = JsonDocument.Parse(rawJson, parseOptions);
            var root = doc.RootElement;

            var interpretation = root.GetProperty("interpretation").GetString() ?? "";
            var moodName = root.GetProperty("mood").GetString() ?? "Peaceful";

            var symbols = new List<string>();
            foreach (var element in root.GetProperty("symbols").EnumerateArray())
                symbols.Add(element.GetString() ?? "");

            return new AnalysisResponseDto
            {
                Title = string.Empty,
                Interpretation = interpretation,
                MoodName = moodName,
                Strategy = "freudian",
                Symbols = symbols,
            };
        }
    }
}
