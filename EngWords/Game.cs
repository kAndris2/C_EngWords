using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace EngWords
{
    class Game
    {
        const String SKIP = "skip";
        const String EXIT = "-1";
        String HINT = "[INFO]: How does it work?\n" +
                      $"\t- Type '{SKIP}' to skip the current round.\n" +
                      $"\t- Type '{EXIT}' to stop the game.\n";

        readonly DataManager data;
        readonly Evaluator evaluator;
        readonly ConsoleLogger _logger;
        readonly Random random = new Random();

        public Game(ConsoleLogger logger)
        {
            data = new DataManager();
            evaluator = new Evaluator(data);
            _logger = logger;
        }

        public void Start(bool mode)
        {
            int maxWords = StartQuestion();
            Dictionary<string, List<string>> words = GetRandomPairs(maxWords);
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>
            {
                {"Success", new Dictionary<string, string>()},
                {"Failed", new Dictionary<string, string>()},
                {"Skipped", new Dictionary<string, string>()}
            };

            result = mode == true ? GuessEnglishWordsMeanings(words, result) : SayInEnglish(words, result);

            Evaluate(result, maxWords, mode);
        }

        int StartQuestion()
        {
            Console.WriteLine($"How much word do you want to play with? Max: {data.GetCount()}");
            string answer = Console.ReadLine();

            if (Int32.TryParse(answer, out int num))
            {
                if (num < 1 || num > data.GetCount())
                    throw new AggregateException($"Invalid index! - ('{num}')");
                else
                {
                    Console.Clear();
                    return num;
                }
            }
            else
                throw new AggregateException($"The entered value is not a number! - ('{answer}')");
        }

        void Evaluate(Dictionary<string, Dictionary<string, string>> stats, int max, bool gameType)
        {
            Console.Clear();
            string[] titles = new string[3] { "Success", "Failed", "Skipped"};
            int percent;

            foreach(string title in titles)
            {
                if (stats[title].Count >= 1)
                {
                    percent = (int)Math.Round((double)(100 * stats[title].Count) / max);
                    ConsoleColor color = ConsoleColor.White;

                    switch(title.ToUpper())
                    {
                        case "SUCCESS": color = ConsoleColor.Green; break;
                        case "FAILED": color = ConsoleColor.Red; break;
                        case "SKIPPED": color = ConsoleColor.Yellow; break;
                    }

                    Console.ForegroundColor = color;
                    Console.Write($"[{title.ToUpper()}]: ");
                    Console.ResetColor();
                    Console.Write($"{stats[title].Count}/{max} - {percent}%\n");
                    evaluator.ShowResults(
                        stats[title],
                        gameType,
                        title == "Failed" ? true : false
                    );
                }
            }
        }

        Dictionary<string, Dictionary<string, string>> SayInEnglish(Dictionary<string, List<string>> words, Dictionary<string, Dictionary<string, string>> stats)
        {
            foreach (KeyValuePair<string, List<string>> pair in words)
            {
                ShowCurrentScores(stats, words);
                int rIndex = random.Next(pair.Value.Count);
                string rWord = pair.Value[rIndex];

                GameQuestion("How do you say in English the", rWord);
                string input = Console.ReadLine();

                if (input == EXIT)
                {
                    break;
                }
                else if (input == "skip")
                {
                    stats["Skipped"].Add(
                        rWord,
                        "(-skipped-)"
                    );
                }
                else if (pair.Key.Equals(input))
                {
                    stats["Success"].Add(
                        rWord,
                        input
                    );
                }
                else
                {
                    stats["Failed"].Add(
                        rWord,
                        input
                    );
                }
                Console.Clear();
            }
            return stats;
        }

        Dictionary<string, Dictionary<string, string>> GuessEnglishWordsMeanings(Dictionary<string, List<string>> words, Dictionary<string, Dictionary<string, string>> stats)
        {
            foreach(KeyValuePair<string, List<string>> pair in words)
            {
                ShowCurrentScores(stats, words);

                GameQuestion("What does it mean the", pair.Key);
                string input = Console.ReadLine();

                if(input == EXIT)
                {
                    break;
                }
                else if(input == "skip")
                {
                    stats["Skipped"].Add(
                        pair.Key,
                        "(-skipped-)"
                    );
                }
                else if(pair.Value.Contains(input))
                {
                    stats["Success"].Add(
                        pair.Key,
                        input
                    );
                }
                else
                {
                    stats["Failed"].Add(
                        pair.Key,
                        input
                    );
                }
                Console.Clear();
            }
            return stats;
        }

        void ShowCurrentScores(Dictionary<string, Dictionary<string, string>> stats, Dictionary<string, List<string>> words)
        {
            String GetHyphens(int basic, int maxLength, Dictionary<string, Dictionary<string, string>> stats)
            {
                int max = (stats["Success"].Count == 0 ? 1 : stats["Success"].Count.ToString().Length) +
                          (stats["Failed"].Count == 0 ? 1 : stats["Failed"].Count.ToString().Length) +
                          (stats["Skipped"].Count == 0 ? 1 : stats["Skipped"].Count.ToString().Length) +
                          basic + maxLength;
                string text = "";
                for (int i = 0; i < max; i++)
                {
                    text += "-";
                }
                return text;
            }

            string hyphens = GetHyphens(41, words.Count.ToString().Length * 3, stats); //41 = static characters count

            _logger.Info(HINT);

            Console.WriteLine(hyphens);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SUCCESS]: ");
            Console.ResetColor();
            Console.Write($"{stats["Success"].Count}/{words.Count}  ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[FAILED]: ");
            Console.ResetColor();
            Console.Write($"{stats["Failed"].Count}/{words.Count}  ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[SKIPPED]: ");
            Console.ResetColor();
            Console.Write($"{stats["Skipped"].Count}/{words.Count} |\n");

            Console.WriteLine(hyphens);

            Console.WriteLine();
        }

        Dictionary<string, List<string>> GetRandomPairs(int max)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> origin = new Dictionary<string, List<string>>(data.GetWords());
            
            while (result.Count != max)
            {
                int index = random.Next(origin.Count);

                result.Add(
                    origin.Keys.ElementAt(index),
                    origin.Values.ElementAt(index)
                );

                origin.Remove(
                    origin.Keys.ElementAt(index)
                );
            }
            return result;
        }

        void GameQuestion(string question, string word)
        {
            Console.Write($"{question} '");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(word);
            Console.ResetColor();
            Console.Write("'?");
            Console.WriteLine();
        }
    }
}
