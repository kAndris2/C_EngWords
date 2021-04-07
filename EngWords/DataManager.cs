using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EngWords
{
    class DataManager
    {
        const String WORDS = "words.txt";
        const String SEPARATOR = " - ";

        Dictionary<string, List<string>> words;

        public DataManager()
        {
            UpdateWords();
        }

        public void Modify(string key, string newPair)
        {
            DeleteWords(
                new List<string> { key }
            );

            string[] temp = newPair.Split('-');

            StoreNewWords(
                new Dictionary<string, List<string>>
                {
                    {temp[0], new List<string>(temp[1].Split('/'))}
                }
            );
        }

        public KeyValuePair<string, List<string>> GetPair(string key, bool mode = false) 
        {
            KeyValuePair<string, List<string>> pair;
            if (!mode)
            {
                pair = words.FirstOrDefault(w => w.Key == key);
                if (pair.Key != "")
                    return pair;
            }
            else
            {
                foreach(KeyValuePair<string, List<string>> item in words)
                {
                    if (item.Value.Contains(key))
                        return item;
                }
            }
            throw new AggregateException($"Invalid key! - ('{key}')");
        }

        public int GetCount() { return words.Count; }

        public Dictionary<string, List<string>> GetWords() 
        {
            if (words.Count >= 1)
                return words;
            else
                throw new AggregateException("You have no any saved words yet!");
        }

        public void DeleteWords(List<string> keys)
        {
            string text = "";

            foreach (KeyValuePair<string, List<string>> word in words)
            {
                if (!keys.Contains(word.Key))
                {
                    text += FormatRow(word);
                }
            }
            File.WriteAllText(WORDS, text);
            UpdateWords();
        }

        public List<string> StoreNewWords(Dictionary<string, List<string>> newWords)
        {
            List<string> errors = new List<string>();

            foreach(KeyValuePair<string, List<string>> word in newWords)
            {
                if (!IsExisting(word.Key))
                {
                    File.AppendAllText(
                        WORDS, 
                        FormatRow(word)
                    );
                }
                else
                    errors.Add(word.Key);
            }
            UpdateWords();

            return errors;
        }

        public Boolean IsExisting(string word)
        {
            return words.ContainsKey(word);
        }

        Dictionary<string, List<string>> ReadWords()
        {
            string[] rawWords = ReadFile(WORDS);
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (string row in rawWords)
            {
                string[] temp = row.Split(SEPARATOR);

                result.Add(
                    temp[0],
                    new List<string>(temp[1].Split('/'))
                );
            }
            return result;
        }

        String[] ReadFile(string filename)
        {
            if (File.Exists(filename))
            {
                return File.ReadAllLines(filename);
            }
            else
            {
                FileStream fs = File.Create(filename);
                fs.Close();
                return new string[0];
            }
        }

        String FormatRow(KeyValuePair<string, List<string>> row)
        {
            return $"{row.Key} - {String.Join('/', row.Value)}\n";
        }

        void UpdateWords()
        {
            words = ReadWords();
        }
    }
}
