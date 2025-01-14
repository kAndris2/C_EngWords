﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngWords
{
    class OptionMenu : Menu
    {
        const String HINT = "How it's working?\n" +
                            "\t- Save a new word separated from it's meaning by a hyphen: 'dog-kutya'\n" +
                            "\t- You can add additional meanings to the word like this: 'dog-kutya/eb'\n";

        protected override void ShowMenu()
        {
            Console.WriteLine("[OPTION MENU]:\n");
            List<string> options = new List<string>
            {
                "Add new word(s)",
                "Show all words",
                "Delete word(s)",
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
                    List<string> errors = _data.StoreNewWords(newWords);

                    if(newWords.Count - errors.Count >= 1)
                    {
                        _logger.Success($"You have successfully stored {newWords.Count - errors.Count}/{newWords.Count} words!");
                    }
                    if(errors.Count >= 1)
                    {
                        _logger.Error($"These words failed: {String.Join(',', errors)}");
                    }
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
                    GetIndexes("\nEnter the indexes separated by commas that you want to delete:")
                );

                _data.DeleteWords(keys);

                Console.Clear();
                _logger.Success($"You have successfully deleted '{keys.Count}' item(s): {string.Join(',', keys)}");
                WaitToKey();

                return true;
            }
            else if (input == "4")
            {
                Console.Clear();

                ListAllWords();
                string key = GrabKeys(
                    GetIndexes("\nPlease type the word's index that you want to modify:")
                )[0];

                Console.Clear();
                KeyValuePair<string, List<string>> pair = _data.GetPair(key);
                string newPair = GetNewPair(key, pair);
                _data.Modify(key, newPair);

                Console.Clear();
                _logger.Success(
                    $"You have successfully modified:\n" +
                    $"'{pair.Key}: {string.Join(',', pair.Value)}' to '{newPair}'"
                );
                WaitToKey();

                return true;
            }
            else
                throw new KeyNotFoundException($"There is no such option! - ('{input}')");
        }

        String GetNewPair(string key, KeyValuePair<string, List<string>> pair)
        {
            Console.WriteLine($"Modifying: '{pair.Key}: {string.Join(',', pair.Value)}'\n");
            _logger.Info(HINT);
            string newPair = Console.ReadLine();

            if (newPair.Contains('-'))
            {
                return newPair;
            }
            else
                throw new AggregateException($"The word/meaning pairs must have hyphen!");
        }

        List<int> GetIndexes(string message)
        {
            List<int> Validate(string[] indexes)
            {
                List<int> result = new List<int>();
                foreach(string index in indexes)
                {
                    if (int.TryParse(index, out int num))
                    {
                        num--;
                        if (num <= _data.GetCount() - 1)
                        {
                            result.Add(num);
                        }
                        else
                            throw new AggregateException($"This index is invalid! - ('{num}')");
                    }
                    else
                        throw new AggregateException($"This value '{index}' is not a number!");
                }
                return result;
            }

            Console.WriteLine(message);

            string[] indexes = Console.ReadLine().Split(',');
            List<int> validIndexes = Validate(indexes);

            return validIndexes;
        }

        List<string> GrabKeys(List<int> indexes)
        {
            List<string> keys = new List<string>();
            int i = 0;

            foreach (KeyValuePair<string, List<string>> word in _data.GetWords())
            {
                if (indexes.Contains(i))
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
            foreach (KeyValuePair<string, List<string>> word in _data.GetWords())
            {
                i++;
                Console.WriteLine($"{i}. - {word.Key}: {string.Join(',', word.Value)}");
            }
        }

        Dictionary<string, List<string>> GetNewWords()
        {
            _logger.Info(HINT);
            string exit = "-1";
            Dictionary<string, List<string>> newWords = new Dictionary<string, List<string>>();
            string input = "";
            string error = "";

            while(input != exit)
            {
                if(error != "")
                {
                    _logger.Error(error);
                    error = "";
                }

                Console.WriteLine($"Please give me a new word/meaning pair, or type '{exit}' to exit.");
                input = Console.ReadLine();

                if (input != exit && input.Contains('-'))
                {
                    string[] temp = input.Split('-');

                    if (!_data.IsExisting(temp[0]))
                    {
                        if (!newWords.ContainsKey(temp[0]))
                        {
                            newWords.Add(
                                temp[0],
                                new List<string>(temp[1].Split('/'))
                            );
                        }
                        else
                            error = $"This word '{temp[0]}' has already been added!";
                    }
                    else
                        error = $"This word '{temp[0]}' has already been stored!";
                }
                else
                    error = "This word/meaning pair doesn't containing hyphen!";
            }
            return newWords;
        }
    }
}