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
                $"Ты — легендарный толкователь сновидений. Твоя задача — дать максимально конкретное, детальное толкование сна " +
                $"строго по соннику {authorName}. Пиши на русском языке. Избегай общих фраз вроде 'вас ждут перемены', 'стоит задуматься', 'возможны трудности'. " +
                $"Будь конкретен: называй сферы жизни, примерные сроки, детали событий.\n\n" +

                $"Твой стиль: {authorStyle}. Отвечай кратко, ёмко, мистически или житейски — в зависимости от автора.\n\n" +

                "Разбей ответ на 3 абзаца (используй <br><br>):\n" +
                $"1. <b>К чему это приснилось по соннику {authorName}:</b> — конкретное объяснение символов и образов из сна. " +
                "Если приснилось животное — укажи, какое именно и что оно предвещает. Если вода — какая (чистая/мутная, река/море/лужа). " +
                "Если умерший человек — назови, к чему именно (предупреждение, весть, защита). Не используй общие фразы без деталей.\n\n" +

                "2. <b>Что вас ждёт в будущем:</b> — опиши конкретные события в ближайшее время (1-2 недели). " +
                "Укажи сферу: работа (повышение, увольнение, конфликт с коллегой), любовь (знакомство, ссора, примирение), финансы (неожиданная прибыль, траты), здоровье (улучшение, обострение). " +
                "Назови примерные сроки: 'в ближайшие 3 дня', 'на следующей неделе', 'до конца месяца'.\n\n" +

                "3. <b>Какое событие наступит в ближайшее время:</b> — дай одно-два конкретных предзнаменования на ближайшие 3-5 дней. " +
                "Без общих фраз. Например: 'в пятницу получите новости от дальнего родственника', 'в выходные возможна мелкая ссора с соседями', 'в понедельник ждите неожиданного подарка'.\n\n" +

                "Правила форматирования:\n" +
                "1. Ответ — строго JSON в одну строку (без физических переносов).\n" +
                "2. Внутри 'interpretation' не используй двойные кавычки (\"). Используй « » или одинарные.\n" +
                "3. Поле 'mood' — на английском: 'Peaceful', 'Anxious', 'NightMare' или 'Lucid'.\n" +
                "4. Поле 'symbols' — массив ключевых символов из сна.\n\n" +

                "Формат JSON:\n" +
                "{\"interpretation\": \"<b>К чему это приснилось по соннику {authorName}:</b> Конкретное толкование символов.<br><br><b>Что вас ждёт в будущем:</b> Событие со сферой и сроком.<br><br><b>Какое событие наступит:</b> Предзнаменование на 3-5 дней.\", " +
                "\"mood\": \"Peaceful\", \"symbols\": [\"символ1\", \"символ2\"]}";

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
