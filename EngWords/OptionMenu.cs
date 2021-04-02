using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngWords
{
    class OptionMenu : Menu
    {
        protected DataManager data = new DataManager();

        protected override void ShowMenu()
        {
            Console.WriteLine("[OPTION MENU]:\n");
            List<string> options = new List<string>
            {
                "Add new word(s).",
                "Show all words.",
                "Delete word(s).",
                "Modify word"
            };

            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"({i + 1}). - {options[i]}");
            Console.WriteLine("\n(0). - Back to Main menu.");
        }

        protected override bool MenuFunctions()
        {
            Console.WriteLine("\nPlease enter an index to choose a function:");
            string input = Console.ReadLine();

            if (input == "0")
            {
                return false;
            }
            else if (input == "1")
            {
                Console.Clear();

                Dictionary<string, List<string>> newWords = GetNewWords();
                if (newWords.Count == 0)
                {
                    throw new AggregateException("You haven't added new words!");
                }
                else
                {
                    Console.Clear();
                    List<string> errors = data.StoreNewWords(newWords);

                    Console.WriteLine(
                        $"You have successfully stored {newWords.Count - errors.Count}/{newWords.Count} words!\n" +
                        (errors.Count >= 1 ? $"These words failed: {String.Join(',', errors)}" : "")
                    );
                    WaitToKey();
                }

                return true;
            }
            else if (input == "2")
            {
                Console.Clear();

                ListAllWords();
                WaitToKey();

                return true;
            }
            else if (input == "3")
            {
                Console.Clear();

                ListAllWords();
                List<string> keys = GrabKeys(
                    GetIndexes()
                );

                data.DeleteWords(keys);

                Console.Clear();
                Console.WriteLine($"You have successfully deleted '{keys.Count}' item(s): {string.Join(',', keys)}");
                WaitToKey();

                return true;
            }
            else
                throw new KeyNotFoundException($"There is no such option! - ('{input}')");
        }

        List<int> GetIndexes()
        {
            Console.WriteLine("\nEnter the indexes separated by commas that you want to delete:");
            return Console.ReadLine().Split(',').Select(Int32.Parse).ToList();
        }

        List<string> GrabKeys(List<int> indexes)
        {
            List<string> keys = new List<string>();
            int i = 0;

            foreach (KeyValuePair<string, List<string>> word in data.GetWords())
            {
                if (indexes.Contains(i + 1))
                {
                    keys.Add(word.Key);
                }
                i++;
            }
            return keys;
        }

        void ListAllWords()
        {
            int i = 0;
            foreach (KeyValuePair<string, List<string>> word in data.GetWords())
            {
                i++;
                Console.WriteLine($"{i}. - {word.Key}: {string.Join(',', word.Value)}");
            }
        }

        Dictionary<string, List<string>> GetNewWords()
        {
            String SetError(string error)
            {
                return $"[ERROR]: {error}";
            }

            Console.WriteLine(
                "[INFO]: How it's working?\n" +
                "- Save a new word separated from it's meaning by a hyphen: 'dog-kutya'\n" +
                "- You can add additional meanings to the word like this: 'dog-kutya/eb'\n"
            );
            string exit = "-1";
            Dictionary<string, List<string>> newWords = new Dictionary<string, List<string>>();
            string input = "";
            string error = "";

            while(input != exit)
            {
                if(error != "")
                {
                    Console.WriteLine(error);
                    error = "";
                }

                Console.WriteLine($"Please give me a new word/meaning pair, or type '{exit}' to exit.");
                input = Console.ReadLine();

                if (input != exit && input.Contains('-'))
                {
                    string[] temp = input.Split('-');

                    if (!newWords.ContainsKey(temp[0]))
                    {
                        newWords.Add(
                            temp[0],
                            new List<string>(temp[1].Split('/'))
                        );
                    }
                    else
                    {
                        error = SetError($"This word '{temp[0]}' has already been added!");
                    }
                }
                else
                    error = SetError("This word/meaning pair doesn't containing hyphen!");
            }
            return newWords;
        }
    }
}
