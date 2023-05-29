using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppLibrary
{
    public class WordCounterLib
    {
        // Разделительные символы для токенизации текста
        char[] separators = new char[] { ' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r', '\t' };

        private Dictionary<string, int> CountingWords(string content)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Разделение текста на слова
            string[] words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Подсчет количества употреблений каждого слова
            var wordCount = new Dictionary<string, int>();
            foreach (string word in words)
            {
                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }

            // Сортировка слов по количеству употреблений в порядке убывания
            var sortedWords = wordCount.OrderByDescending(w => w.Value).ToDictionary(k => k.Key, v => v.Value);

            stopwatch.Stop();
            Console.WriteLine($"Время обработки приватного метода: {stopwatch.ElapsedMilliseconds}");

            return sortedWords;
        }

        public Dictionary<string, int> CountingWordsParallel(string content)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Разделение текста на слова
            string[] words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Подсчет количества употреблений каждого слова
            var wordCountParallel = new ConcurrentDictionary<string, int>();
            Parallel.ForEach(words, w =>
            {
                wordCountParallel.AddOrUpdate(w, 1, (_, count) => count + 1);
            });

            // Сортировка слов по количеству употреблений в порядке убывания
            var sortedWords = wordCountParallel.OrderByDescending(w => w.Value).ToDictionary(k => k.Key, v => v.Value);

            stopwatch.Stop();
            Console.WriteLine($"Время обработки публичного, многопоточного метода: {stopwatch.ElapsedMilliseconds}");

            return sortedWords;
        }
    }
}
