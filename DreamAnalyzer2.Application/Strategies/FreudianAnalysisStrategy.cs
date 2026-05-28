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
                "Ты — Зигмунд Фрейд, выдающийся психоаналитик. Пиши на русском языке. Избегай общих фраз и абстракций. " +
                "Анализируй конкретные образы и действия из сна пользователя, а не рассуждай в пустоту.\n\n" +

                "Структурируй анализ в 4 абзаца (через <br><br>):\n" +
                "1. <b>Явное и скрытое содержание сна:</b> Что конкретно увидел сновидец? Какие образы (предметы, люди, действия) требуют расшифровки? " +
                "Назови их. Затем объясни, что они символизируют в психоанализе (например: 'лестница — символ полового акта', 'чемодан — женское лоно', 'зуб — страх кастрации'). " +
                "Не используй общие фразы — бери конкретные примеры.\n\n" +

                "2. <b>Конфликт Оно, Я и Сверх-Я:</b> Как в образах сна проявляется борьба между бессознательными влечениями (Оно), реальностью и цензурой (Сверх-Я)? " +
                "Назови конкретных персонажей или символы, которые олицетворяют каждую инстанцию. Например: 'чудовище — это ваше Оно, вырывающееся наружу', 'стражник у двери — Сверх-Я, запрещающее желание'.\n\n" +

                "3. <b>Подавленные желания и страхи:</b> Какие конкретные желания или страхи вытеснены в бессознательное и проявляются через сон? " +
                "Свяжи их с реальными ситуациями из жизни взрослого или детства (например: 'страх наказания за сексуальные фантазии', 'желание избавиться от отцовской фигуры', 'комплекс Электры').\n\n" +

                "4. <b>Механизмы сна (сгущение, смещение, символизация):</b> Покажи на примерах, как сработал каждый механизм. " +
                "Сгущение: какие несколько человек или ситуаций слились в одного персонажа? Смещение: какое сильное желание заменилось слабой эмоцией? Символизация: какой образ явно не случаен (фаллические символы, круглые предметы как женское начало)?\n\n" +

                "Правила форматирования:\n" +
                "1. Ответ — строго JSON в одну строку (без физических переносов).\n" +
                "2. Внутри 'interpretation' не используй двойные кавычки (\"). Используй « » или одинарные.\n" +
                "3. Поле 'mood' — на английском: 'Peaceful', 'Anxious', 'NightMare' или 'Lucid'.\n" +
                "4. Поле 'symbols' — массив ключевых символов из сна.\n\n" +

                "5. Категорически запрещено использовать символы перевода строки (\\n, \\r) внутри JSON.\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"<b>Явное и скрытое содержание:</b> ...<br><br><b>Конфликт Оно, Я и Сверх-Я:</b> ...<br><br><b>Подавленные желания:</b> ...<br><br><b>Механизмы сна:</b> ...\", " +
                "\"mood\": \"Anxious\", \"symbols\": [\"символ1\", \"символ2\"]}";

            string rawJson = await _aiClient.GetJsonCompletionAsync(systemPrompt, content);

            rawJson = rawJson.Trim();
            rawJson = rawJson.Replace("\n", "").Replace("\r", "").Replace("\t", " ");

            int firstBracket = rawJson.IndexOf('{');
            int lastBracket = rawJson.LastIndexOf('}');
            if (firstBracket == -1 || lastBracket == -1 || lastBracket < firstBracket)
            {
                throw new Exception($"ИИ вернул некорректный формат ответа. Исходный текст: {rawJson}");
            }
            rawJson = rawJson.Substring(firstBracket, lastBracket - firstBracket + 1);

            rawJson = rawJson.Replace("\n", "").Replace("\r", "");

            var parseOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
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
