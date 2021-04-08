using System;
using System.Collections.Generic;
using System.Text;

namespace EngWords
{
    class GameMenu : Menu
    {
        readonly Game game;

        public GameMenu()
        {
            game = new Game(_logger, _data);
        }

        protected override void ShowMenu()
        {
            Console.WriteLine("[GAME MENU]:\n");
            List<string> options = new List<string>
            {
                "Guess the meaning of the English words.",
                "How do you say in English?"
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

                game.Start(true);
                WaitToKey();

                return true;
            }
            else if (input == "2")
            {
                Console.Clear();

                game.Start(false);
                WaitToKey();

                return true;
            }
            else
                throw new KeyNotFoundException($"There is no such option! - ('{input}')");
        }
    }
}
