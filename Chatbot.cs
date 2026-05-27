using System;
using System.Threading;

namespace CyberSecurityChatbot
{
    internal class Chatbot
    {
        private readonly User _user;

        public Chatbot(User user)
        {
            _user = user;
        }

        public static void DisplayAsciiBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            var art = @"   ____ _       _                 ____        _   
  / ___| | ___ | |__   ___  _ __ / ___|  ___ | |_ 
 | |   | |/ _ \| '_ \ / _ \| '__| |  _  / _ \| __|
 | |___| | (_) | |_) | (_) | |  | |_| || (_) | |_ 
  \____|_|\___/|_.__/ \___/|_|   \____| \___/ \__|";
            Console.WriteLine(art);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("     Cybersecurity Awareness Bot");
            Console.WriteLine(new string('═', 54));
            Console.WriteLine();
        }

        public void Run()
        {
            DisplayIntro();
            AskUserName();
            WelcomeUser();
            ShowHelpPrompt();
            MainLoop();
        }

        private static void DisplayIntro()
        {
            PrintBorder("Welcome to the Cybersecurity Awareness Bot");
            TypingWrite("I can answer questions about passwords, phishing, malware, VPNs, and keeping your data safer.");
            Console.WriteLine();
        }

        private void AskUserName()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("What is your name? ");
                Console.ResetColor();
                var input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    _user.Name = input;
                    return;
                }

                TypingWrite("Please enter your name so I can personalize the experience.");
            }
        }

        private void WelcomeUser()
        {
            TypingWrite($"Nice to meet you, {_user.Name}! I’m ready when you are.");
            Console.WriteLine();
        }

        private static void ShowHelpPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Type a question about cybersecurity, or say 'help', 'exit', or 'quit'.");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void MainLoop()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"{_user.Name}> ");
                Console.ResetColor();
                var question = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(question))
                {
                    TypingWrite("I didn't receive a question. Please type something so I can help.");
                    continue;
                }

                if (HandleExit(question))
                {
                    break;
                }

                RespondTo(question);
            }

            TypingWrite("Goodbye! Stay safe online.");
        }

        private static bool HandleExit(string text)
        {
            var command = text.ToLowerInvariant();
            return command is "exit" or "quit" or "bye" or "close";
        }

        private void RespondTo(string text)
        {
            var response = ChatLogic.GetResponse(_user, text);
            TypingWrite(response);
            
            var response = ChatLogic.GetResponse(_user, text);
            TypingWrite(response);
>>>>>>> 431d709 (Add EF Core persistence, entities, README and persistence wiring)
            Console.WriteLine();
        }

        private static void TypingWrite(string text, int delay = 20)
        {
            foreach (var character in text)
            {
                Console.Write(character);
                Thread.Sleep(delay);
            }

            Console.WriteLine();
        }

        private static void PrintBorder(string message)
        {
            var frameLength = message.Length + 6;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(new string('═', frameLength));
            Console.WriteLine($"║  {message}  ║");
            Console.WriteLine(new string('═', frameLength));
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
