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
            var normalized = text.ToLowerInvariant();

            if (normalized.Contains("how are you"))
            {
                TypingWrite("I am fully operational and ready to keep you safe.");
            }
            else if (normalized.Contains("purpose") || normalized.Contains("what do you do") || normalized.Contains("what is your purpose"))
            {
                TypingWrite("My purpose is to help you learn cybersecurity best practices and stay protected online.");
            }
            else if (normalized.Contains("password"))
            {
                TypingWrite("Use a long, unique password for each account, enable multi-factor authentication, and store passwords in a password manager.");
            }
            else if (normalized.Contains("phishing"))
            {
                TypingWrite("Phishing attacks use fake emails and links. Always verify the sender, hover over links, and never share your password.");
            }
            else if (normalized.Contains("vpn"))
            {
                TypingWrite("A VPN helps protect your data on public Wi-Fi by encrypting your connection. Use one from a trusted provider.");
            }
            else if (normalized.Contains("malware") || normalized.Contains("virus") || normalized.Contains("ransomware"))
            {
                TypingWrite("Malware is harmful software. Keep your system updated, install antivirus software, and avoid downloading unknown files.");
            }
            else if (normalized.Contains("two-factor") || normalized.Contains("2fa") || normalized.Contains("multi-factor"))
            {
                TypingWrite("Two-factor authentication adds a second layer of protection beyond your password. Always enable it when available.");
            }
            else if (normalized.Contains("help"))
            {
                TypingWrite("Ask me anything about cybersecurity: password safety, phishing, malware, VPNs, and how to protect your accounts.");
            }
            else
            {
                TypingWrite("That question is new to me. Try asking about passwords, phishing, malware, VPNs, or account safety.");
            }

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
