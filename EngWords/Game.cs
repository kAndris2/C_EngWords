using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngWords
{
    class Game
    {
        const String SKIP = "skip";
        const String EXIT = "-1";
        String HINT = "[INFO]: How does it work?\n" +
                      $"- Type '{SKIP}' to skip the current round.\n" +
                      $"- Type '{EXIT}' to stop the game.\n";

        DataManager data = new DataManager();
        Random random = new Random();

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

            Evaluate(result, maxWords);
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

        void Evaluate(Dictionary<string, Dictionary<string, string>> stats, int max)
        {
            String ListResults(Dictionary<string, string> result)
            {
                string text = "";
                foreach(KeyValuePair<string, string> item in result)
                {
                    text += $"{item.Key} : {item.Value}\n";
                }
                return text;
            }

            Console.Clear();

            if (stats["Success"].Count >= 1)
            {
                Console.WriteLine(
                    $"[SUCCESS]: {stats["Success"].Count}/{max}\n" +
                    $"{ListResults(stats["Success"])}"
                );
            }

            if (stats["Failed"].Count >= 1)
            {
                Console.WriteLine(
                    $"[FAILED]: {stats["Failed"].Count}/{max}\n" +
                    $"{ListResults(stats["Failed"])}"
                );
            }

            if (stats["Skipped"].Count >= 1)
            {
                Console.WriteLine(
                    $"[SKIPPED]: {stats["Skipped"].Count}/{max}\n" +
                    $"{ListResults(stats["Skipped"])}"
                );
            }
        }

        Dictionary<string, Dictionary<string, string>> SayInEnglish(Dictionary<string, List<string>> words, Dictionary<string, Dictionary<string, string>> stats)
        {
            foreach (KeyValuePair<string, List<string>> pair in words)
            {
                ShowCurrentScores(stats, words);
                int rIndex = random.Next(pair.Value.Count);
                string rWord = pair.Value[rIndex];

                Console.WriteLine($"How do you say in English the '{rWord}'?");
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

                Console.WriteLine($"What does it mean the '{pair.Key}'?");
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
            Console.WriteLine(HINT);
            Console.WriteLine(
                    $"[SUCCESS]: {stats["Success"].Count}/{words.Count}\t" +
                    $"[FAILED]: {stats["Failed"].Count}/{words.Count}\t" +
                    $"[SKIPPED]: {stats["Skipped"].Count}/{words.Count}\n"
                );
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
    }
}
