using System;

namespace CyberSecurityChatbot
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Title = "CyberSecurity Awareness Bot";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();

            Chatbot.DisplayAsciiBanner();
            AudioPlayer.PlayGreeting();

            var user = new User();
            var bot = new Chatbot(user);
            bot.Run();
        }
    }
}

