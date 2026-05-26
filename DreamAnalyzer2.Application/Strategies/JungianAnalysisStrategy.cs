using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DreamAnalyzer2.Application.Strategies
{
    public class JungianAnalysisStrategy : IAnalysisStrategy
    {
        private readonly IDreamAiClient _aiClient;

        public JungianAnalysisStrategy(IDreamAiClient aiClient)
        {
            _aiClient = aiClient;
        }

        public async Task<AnalysisResponseDto> AnalyzeAsync(string content)
        {
            string systemPrompt =
                "Ты — Карл Густав Юнг, великий аналитический психолог. Твоя задача — провести глубокий, развернутый и " +
                "детальный анализ сновидения пользователя, опираясь на концепции коллективного бессознательного и архетипов. " +
                "Анализ должен быть подробным (минимум 3-4 объемных абзаца) и написан в уважительном, мудром тоне.\n\n" +

                "В тексте анализа (поле 'interpretation') ты ОБЯЗАН отразить следующие аспекты:\n" +
                "1. Коллективное бессознательное: какие общечеловеческие, мифологические или культурные мотивы скрыты во сне.\n" +
                "2. Архетипы: определи, проявляются ли во сне ключевые архетипы — Тень (вытесненная часть личности), Анима/Анимус (внутреннее женское/мужское начало), Персона или Мудрец.\n" +
                "3. Процесс индивидуации: как этот сон помогает сновидцу интегрировать неосознанные части души и двигаться к Самости (целостности).\n" +
                "4. Компенсаторная функция сна: какую однобокость сознания (дневных мыслей) пытается уравновесить этот сон.\n\n" +

                "Правила форматирования ответа:\n" +
                "1. Ты обязан вернуть ответ СТРОГО в формате JSON в одну строку (БЕЗ физических переносов строки через Enter).\n" +
                "2. Внутри текста 'interpretation' тебе КАТЕГОРИЧЕСКИ ЗАПРЕЩЕНО использовать стандартные двойные кавычки \". Для выделения цитат или понятий используй ТОЛЬКО кавычки-ёлочки « » или одинарные кавычки ' '.\n" +
                "3. Для разделения аналитического разбора на абзацы внутри поля 'interpretation' ОБЯЗАТЕЛЬНО используй теги <br><br>.\n" +
                "4. Используй английские названия Mood: 'Peaceful', 'Anxious', 'NightMare', 'Lucid'.\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"Первый абзац анализа с упоминанием архетипа «Тень»...<br><br>Второй абзац про Самость...\", \"mood\": \"Peaceful\", \"symbols\": [\"Старик\", \"Море\"]}";

            string rawJson = await _aiClient.GetJsonCompletionAsync(systemPrompt, content);

            int firstBracket = rawJson.IndexOf('{');
            int lastBracket = rawJson.LastIndexOf('}');
            if (firstBracket == -1 || lastBracket == -1 || lastBracket < firstBracket)
            {
                throw new Exception($"ИИ вернул некорректный формат ответа для стратегии Юнга. Исходный текст: {rawJson}");
            }
            rawJson = rawJson.Substring(firstBracket, lastBracket - firstBracket + 1);

            var parseOptions = new JsonDocumentOptions { AllowTrailingCommas = true };
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
                Strategy = "jungian", 
                Symbols = symbols,
            };
        }
    }
}
