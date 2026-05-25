using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Domain.ValueObjects;
using Iveonik.Stemmers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Strategies
{
    public class SymbolStrategy : IAnalysisStrategy
    {
        private readonly ISymbolRepository _symbolRepository;
        private readonly RussianStemmer _stemmer = new RussianStemmer();

        public SymbolStrategy(ISymbolRepository symbolRepository)
        {
            _symbolRepository = symbolRepository;
        }

        public async Task<AnalysisResponseDto> AnalyzeAsync(string content)
        {
            var allSymbols = await _symbolRepository.GetAllAsync();
            var foundSymbols = new List<DreamSymbol>();
            var foundSymbolsNames = new List<string>();
            var interpretations = new List<string>();
            var moodCounts = new Dictionary<string,  int>();

            var rawWords = content.ToLower().Split(new char[] { ' ', ',', '.', '!', '?', '\n', '\r', ';', ':', '-', '(', ')', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);
            var stemmedWords = new HashSet<string>();
            foreach (var word in rawWords)
            {
                var stemmed = _stemmer.Stem(word);
                stemmedWords.Add(stemmed);
            }

            foreach (var symbol in allSymbols)
            {
                var stemmedSymbol = _stemmer.Stem(symbol.Name.ToLower());

                if (stemmedWords.Contains(stemmedSymbol))
                {
                    foundSymbols.Add(symbol);
                    foundSymbolsNames.Add(symbol.Name);
                    interpretations.Add($"{symbol.Name}: {symbol.Interpretation}");

                    if (!moodCounts.ContainsKey(symbol.Mood))
                        moodCounts[symbol.Mood] = 0;
                    moodCounts[symbol.Mood]++;
                }
            }

            var dominantMood = moodCounts.Count > 0
                ? moodCounts.OrderByDescending(x => x.Value).First().Key
                : "Peaceful";
            var mood = GetMoodByName(dominantMood);
            var interpretation = interpretations.Count > 0
                ? string.Join("; ", interpretations)
                : "No specific symbols found in your dream.";

            return new AnalysisResponseDto
            {
                Title = "",
                Interpretation = interpretation,
                Mood = mood,
                Strategy = "symbols",
                Symbols = foundSymbolsNames
            };
        }

        private Mood GetMoodByName(string moodName)
        {
            return moodName switch
            {
                "Peaceful" => Mood.Peaceful,
                "Anxious" => Mood.Anxious,
                "NightMare" => Mood.NightMare,
                "Lucid" => Mood.Lucid,
                _ => Mood.Peaceful
            };
        }
    }
}
