using System;
using System.Collections.Generic;
using System.Text;

namespace EngWords
{
    class MainMenu : Menu
    {
        protected override void ShowMenu()
        {
            Console.WriteLine("[MAIN MENU]:\n");
            List<string> options = new List<string>
            {
                "Play with words",
                "Word options"
            };

            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"({i + 1}). - {options[i]}");
            Console.WriteLine("\n(0). - Exit.");
        }

        protected override bool MenuFunctions()
        {
            Console.WriteLine("\nPlease enter an index to choose a function:");
            string input = Console.ReadLine();

            if (input == "0")
            {
                Environment.Exit(-1);
                return false;
            }
            else if (input == "1")
            {
                Console.Clear();
                new GameMenu().Start();
                return true;
            }
            else if (input == "2")
            {
                Console.Clear();
                new OptionMenu().Start();
                return true;
            }
            else
                throw new KeyNotFoundException($"There is no such option! - ('{input}')");
        }
    }
}
