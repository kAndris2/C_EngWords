using System;
using System.Collections.Generic;
using System.Text;

namespace EngWords
{
    class OptionMenu : Menu
    {
        protected override void ShowMenu()
        {
            Console.WriteLine("[OPTION MENU]:\n");
            List<string> options = new List<string>
            {
                "Add new word(s).",
                "Show all words.",
                "Delete word(s)."
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
            else
                throw new KeyNotFoundException($"There is no such option! - ('{input}')");
        }
    }
}
