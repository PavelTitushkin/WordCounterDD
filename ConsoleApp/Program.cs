using ConsoleAppLibrary;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;

namespace ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string inputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
            string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt");
            string outputFileParallel = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "outputParallel.txt");
            string outputFileAPI = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "outputAPI.txt");
            var stopwatch = new Stopwatch();

            try
            {
                string content = File.ReadAllText(inputFile, Encoding.UTF8);

                #region Задание 1
                Console.WriteLine("Задание 1\n");

                //Доступ и получение данных из приватного метода
                var classInstance = new WordCounterLib();
                var privateMethod = classInstance.GetType().GetMethod("CountingWords", BindingFlags.NonPublic | BindingFlags.Instance);

                //измерение скорости выполнение метода
                stopwatch.Start();
                var dictionary = privateMethod.Invoke(classInstance, new object[] { content }) as Dictionary<string, int>;
                stopwatch.Stop();
                Console.WriteLine($"Время обработки приватного метода: {stopwatch.ElapsedMilliseconds}");

                // Запись результата в выходной файл
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    foreach (var word in dictionary)
                    {
                        writer.WriteLine($"{word.Key}\t{word.Value}");
                    }
                }
                Console.WriteLine("Результат записан в файл output.txt\n");
                #endregion


                #region Задание2
                Console.WriteLine("Задание 2\n");

                // Вызов публичного метода 
                var wordCount = new WordCounterLib();

                //измерение скорости выполнение метода
                stopwatch.Start();
                var dictionaryParallel = wordCount.CountingWordsParallel(content);
                stopwatch.Stop();
                Console.WriteLine($"Время обработки публичного, многопоточного метода: {stopwatch.ElapsedMilliseconds}");

                // Запись результата в выходной файл
                using (StreamWriter writer = new StreamWriter(outputFileParallel))
                {
                    foreach (var word in dictionaryParallel)
                    {
                        writer.WriteLine($"{word.Key}\t{word.Value}");
                    }
                }
                Console.WriteLine("Результат записан в файл outputParallel.txt\n");
                #endregion

                #region Задание 3
                Console.WriteLine("Задание 3");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7151/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage responce = await client.PostAsJsonAsync("api/WordCounter", content);
                    if (responce.IsSuccessStatusCode)
                    {
                        var dictionaryAPI = await responce.Content.ReadFromJsonAsync<Dictionary<string, int>>();

                        // Запись результата в выходной файл
                        using (StreamWriter writer = new StreamWriter(outputFileAPI))
                        {
                            foreach (var word in dictionaryAPI)
                            {
                                writer.WriteLine($"{word.Key}\t{word.Value}");
                            }
                        }
                        Console.WriteLine("Результат записан в файл outputFileAPI.txt\n");
                    }
                    else
                    {
                        Console.WriteLine("Не удалось получить результат!");
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}