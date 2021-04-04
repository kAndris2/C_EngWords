using System;
using System.Collections.Generic;
using System.Text;

namespace EngWords
{
    class Evaluator
    {
        readonly DataManager data;

        public Evaluator(DataManager data)
        {
            this.data = data;
        }

        public void ShowResults(Dictionary<string, string> result, bool gameType, bool showFailed = false)
        {
            if (gameType)
                PrintResultsForGuessEnglishWords(result, showFailed);
            else
                PrintResultsForGuessHungarianWords(result, showFailed);
        }

        void PrintResultsForGuessEnglishWords(Dictionary<string, string> result, bool showFailed = false)
        {
            foreach (KeyValuePair<string, string> item in result)
            {
                KeyValuePair<string, List<string>> origin = data.GetPair(item.Key);

                Console.Write($"{item.Key}:");
                foreach (string word in origin.Value)
                {
                    if (word.Equals(item.Value))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($" {word}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($" {word}");
                    }
                }
                if (showFailed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" {item.Value}");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        void PrintResultsForGuessHungarianWords(Dictionary<string, string> result, bool showFailed = false)
        {
            foreach (KeyValuePair<string, string> item in result)
            {
                KeyValuePair<string, List<string>> origin = data.GetPair(item.Key, true);

                Console.Write($"{item.Key}:");

                if (showFailed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($" {origin.Key} ");
                    Console.ResetColor();
                    Console.Write("-");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" {item.Value}");
                    Console.ResetColor();
                }
                else if (origin.Value.Contains(item.Key))
                {
                    if (!item.Value.Contains("skip"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($" {item.Value}");
                        Console.ResetColor();
                    }
                    else
                        Console.Write($" {origin.Key}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
