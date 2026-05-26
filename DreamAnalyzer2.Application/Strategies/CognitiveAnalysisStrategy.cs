using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DreamAnalyzer2.Application.Strategies
{
    public class CognitiveAnalysisStrategy : IAnalysisStrategy
    {
        private readonly IDreamAiClient _aiClient;

        public CognitiveAnalysisStrategy(IDreamAiClient aiClient)
        {
            _aiClient = aiClient;
        }

        public async Task<AnalysisResponseDto> AnalyzeAsync(string content)
        {
            string systemPrompt =
                "Ты — эксперт в когнитивно-поведенческой психотерапии (КПТ). Твоя задача — провести развернутый " +
                "когнитивный анализ сновидения пользователя, основываясь на 'гипотезе непрерывности' (сон как продолжение дневного опыта).\n\n" +

                "В тексте анализа (поле 'interpretation') ты ОБЯЗАН подробно отразить следующие аспекты:\n" +
                "1. Эмоциональный остаток: какие дневные стрессы, тревоги, дедлайны или нерешенные проблемы проецируются в сюжет этого сна.\n" +
                "2. Когнитивные искажения: определи, какие ошибки мышления видны в поведении сновидца во сне (например, катастрофизация «всё пропало», черно-белое мышление, чтение мыслей других персонажей).\n" +
                "3. Поведенческие паттерны: как сновидец реагирует на угрозы во сне (бегство, замирание, борьба) и как это отражает его реальные стратегии преодоления трудностей (копинг-стратегии).\n" +
                "4. Практические КПТ-рекомендации: дай 2-3 конкретных упражнения для работы с этой тревогой в реальной жизни (техники заземления, когнитивный рефрейминг или ведение дневника мыслей).\n" +
                "Анализ должен быть подробным (3-4 объемных абзаца) и прагматичным.\n\n" +

                "Правила форматирования ответа:\n" +
                "1. Ты обязан вернуть ответ СТРОГО в формате JSON в одну строку (БЕЗ физических переносов строки через Enter).\n" +
                "2. Внутри текста 'interpretation' тебе КАТЕГОРИЧЕСКИ ЗАПРЕЩЕНО использовать стандартные двойные кавычки \". Для выделения цитат или понятий используй ТОЛЬКО кавычки-ёлочки « » или одинарные кавычки ' '.\n" +
                "3. Для разделения аналитического разбора на абзацы внутри поля 'interpretation' ОБЯЗАТЕЛЬНО используй теги <br><br>.\n" +
                "4. Используй английские названия Mood: 'Peaceful', 'Anxious', 'NightMare', 'Lucid'.\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"Первый абзац анализа дневных триггеров...<br><br>Второй абзац про когнитивные искажения...\", \"mood\": \"Anxious\", \"symbols\": [\"Экзамен\", \"Часы\"]}";

            string rawJson = await _aiClient.GetJsonCompletionAsync(systemPrompt, content);

            int firstBracket = rawJson.IndexOf('{');
            int lastBracket = rawJson.LastIndexOf('}');
            if (firstBracket == -1 || lastBracket == -1 || lastBracket < firstBracket)
            {
                throw new Exception($"ИИ вернул некорректный формат ответа для КПТ-стратегии. Исходный текст: {rawJson}");
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
                Strategy = "cognitive", 
                Symbols = symbols,
            };
        }
    }
}
