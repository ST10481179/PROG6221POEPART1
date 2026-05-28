using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using CyberSecurityChatbot.Data;
using CyberSecurityChatbot.Entities;

namespace CyberSecurityChatbot
{
    internal static class Program
    {
        [SupportedOSPlatform("windows")]
        [STAThread]
        private static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=chatbot.db")
                .Options;

            var user = LoadUser(options);

            if (args is null || args.Length == 0 || args[0].ToLowerInvariant() == "-gui")
            {
                var app = new Application();
                var main = new MainWindow(user);
                app.Run(main);
                SaveUser(options, user);
                return;
            }

            if (args[0].ToLowerInvariant() == "-console")
            {
                Console.Title = "CyberSecurity Awareness Bot";
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.Clear();
                Chatbot.DisplayAsciiBanner();
                AudioPlayer.PlayGreeting();

                var bot = new Chatbot(user);
                bot.Run();
                SaveUser(options, user);
                return;
            }

            var guiApp = new Application();
            guiApp.Run(new MainWindow(user));
            SaveUser(options, user);
        }

        private static User LoadUser(DbContextOptions<AppDbContext> options)
        {
            using var db = new AppDbContext(options);
            db.Database.Migrate();

            var saved = db.Users.AsNoTracking().FirstOrDefault();
            if (saved == null)
            {
                saved = new UserEntity { Name = string.Empty };
                db.Users.Add(saved);
                db.SaveChanges();
            }

            var user = new User
            {
                Name = saved.Name ?? string.Empty
            };

            user.RestoreState(saved.Memory, saved.Interests, saved.FavoriteTopic ?? string.Empty, saved.LastTopic ?? string.Empty);

            return user;
        }

        private static void SaveUser(DbContextOptions<AppDbContext> options, User user)
        {
            using var db = new AppDbContext(options);
            var existing = db.Users.FirstOrDefault();
            if (existing == null)
            {
                existing = new UserEntity();
                db.Users.Add(existing);
            }

            existing.Name = user.Name ?? string.Empty;
            existing.LastTopic = user.LastTopic ?? string.Empty;
            existing.FavoriteTopic = user.FavoriteTopic ?? string.Empty;
            existing.Memory = new System.Collections.Generic.List<string>(user.GetMemoryList());
            existing.Interests = new System.Collections.Generic.List<string>(user.GetInterestsList());

            db.SaveChanges();
        }
    }
}

