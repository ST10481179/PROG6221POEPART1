using System;
using System.Runtime.Versioning;

namespace CyberSecurityChatbot
{
    internal static class Program
    {
        [SupportedOSPlatform("windows")]
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

