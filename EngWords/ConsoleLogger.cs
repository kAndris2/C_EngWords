using System;
using System.Collections.Generic;
using System.Text;

namespace EngWords
{
    class ConsoleLogger
    {
        public void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[SUCCESS]: ");
            Console.ResetColor();
            Console.Write(message);
            Console.WriteLine();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR]: ");
            Console.ResetColor();
            Console.Write(message + "\n");
            Console.WriteLine();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[INFO]: ");
            Console.Write(message);
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
