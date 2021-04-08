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
            words = ReadWords();
        }

        public void Modify(string key, string newPair)
        {
            DeleteWords(
                new List<string> { key },
                false
            );

            string[] temp = newPair.Split('-');

            StoreNewWords(
                new Dictionary<string, List<string>>
                {
                    {temp[0], new List<string>(temp[1].Split('/'))}
                },
                false
            );

            SaveToFile();
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

        public void DeleteWords(List<string> keys, bool saveToFile = true)
        {
            foreach(string key in keys)
            {
                words.Remove(key);
            }

            if (saveToFile)
                SaveToFile();
        }

        public List<string> StoreNewWords(Dictionary<string, List<string>> newWords, bool saveToFile = true)
        {
            List<string> errors = new List<string>();

            foreach(KeyValuePair<string, List<string>> word in newWords)
            {
                if (!IsExisting(word.Key))
                {
                    words.Add(
                        word.Key,
                        word.Value
                    );
                }
                else
                    errors.Add(word.Key);
            }

            if (saveToFile)
                SaveToFile();

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

        void SaveToFile()
        {
            String FormatRow(KeyValuePair<string, List<string>> row)
            {
                return $"{row.Key} - {String.Join('/', row.Value)}\n";
            }

            string text = "";
            foreach(var word in words)
            {
                text += FormatRow(word);
            }
            File.WriteAllText(WORDS, text);
        }
    }
}
