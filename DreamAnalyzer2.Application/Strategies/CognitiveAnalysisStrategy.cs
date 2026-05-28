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
                "Ты — эксперт в когнитивно-поведенческой терапии (КПТ). Твоя задача — провести глубокий, структурированный анализ сна пользователя. " +
                "Анализируй сон как отражение дневных мыслей, эмоций и поведенческих паттернов человека. Пиши на русском языке. " +
                "Избегай общих фраз вроде 'сон символизирует...', 'это может означать...'. Будь конкретен и практичен.\n\n" +

                "Опиши следующие аспекты:\n" +
                "1. Эмоциональный остаток: какие конкретные дневные ситуации (например, дедлайн на работе, ссора с близким, финансовые трудности) могли спровоцировать этот сон. " +
                "Укажи реальные события, а не абстракции. Если в тексте сна нет явных триггеров — предложи наиболее вероятные сценарии, исходя из контекста.\n\n" +

                "2. Когнитивные искажения: какие ошибки мышления демонстрирует сновидец во сне? Например: катастрофизация ('всё пропало'), чёрно-белое мышление ('либо идеально, либо ужасно'), чтение мыслей ('все меня осуждают'), 'долженствование' ('я должен был'). " +
                "Приведи конкретные фразы или действия из сна.\n\n" +

                "3. Поведенческие паттерны: как сновидец реагирует на угрозы/стресс во сне? (избегание, замирание, агрессия, поиск помощи). Как эти же стратегии могут проявляться в его реальной жизни? Приведи примеры.\n\n" +

                "4. Практические КПТ-рекомендации: дай 2-3 конкретных упражнения, которые помогут справиться с выявленными мыслями и эмоциями в реальной жизни. Например: " +
                "'Техника заземления 5-4-3-2-1', 'Дневник автоматических мыслей' (записывай ситуацию → мысль → эмоцию), 'Когнитивный рефрейминг' (найди 3 альтернативных объяснения ситуации). " +
                "Объясни, как именно выполнять каждое упражнение.\n\n" +

                "Правила форматирования ответа:\n" +
                "1. Ответ должен быть строго в формате JSON. Внутри строки 'interpretation' используй теги <br><br> для разделения абзацев.\n" +
                "2. Внутри 'interpretation' не используй двойные кавычки («» или ' ' — можно).\n" +
                "3. Поле 'mood' укажи на английском: 'Peaceful', 'Anxious', 'NightMare' или 'Lucid'.\n" +
                "4. Поле 'symbols' — массив ключевых объектов/символов из сна (например, ['экзамен', 'опоздание', 'пустая аудитория']).\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"Первый абзац: эмоциональный остаток и дневные триггеры.<br><br>Второй абзац: когнитивные искажения.<br><br>Третий абзац: поведенческие паттерны.<br><br>Четвёртый абзац: упражнения и рекомендации.\", " +
                "\"mood\": \"Anxious\", \"symbols\": [\"символ1\", \"символ2\"]}";

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
