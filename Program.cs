using System;
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
            // configure EF Core with a local SQLite file
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=chatbot.db")
                .Options;

            User user = new User();

            // create or open DB and load the first user if present
            using (var db = new AppDbContext(options))
            {
                db.Database.EnsureCreated();

                var saved = db.Users.AsNoTracking().FirstOrDefault();
                if (saved == null)
                {
                    saved = new UserEntity { Name = string.Empty };
                    db.Users.Add(saved);
                    db.SaveChanges();
                }

                // map persisted values into runtime User instance
                if (!string.IsNullOrWhiteSpace(saved.Name)) user.Name = saved.Name;
                user.LastTopic = saved.LastTopic ?? string.Empty;
                user.FavoriteTopic = saved.FavoriteTopic ?? string.Empty;

                foreach (var m in saved.Memory ?? new System.Collections.Generic.List<string>())
                {
                    user.Remember(m);
                }

                foreach (var i in saved.Interests ?? new System.Collections.Generic.List<string>())
                {
                    user.RememberInterest(i);
                }
            }

            if (args is null || args.Length == 0 || args[0].ToLowerInvariant() == "-gui")
            {
                var app = new Application();
                var main = new MainWindow(user);
                app.Run(main);

                // save user state after GUI closes
                using (var db = new AppDbContext(options))
                {
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

                // save user state after console run
                using (var db = new AppDbContext(options))
                {
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

                return;
            }

            var guiApp = new Application();
            guiApp.Run(new MainWindow(user));
        [STAThread]
        private static void Main(string[] args)
        {
            // configure EF Core with a local SQLite file
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=chatbot.db")
                .Options;

            User user = new User();

            // create or open DB and load the first user if present
            using (var db = new AppDbContext(options))
            {
                db.Database.EnsureCreated();

                var saved = db.Users.AsNoTracking().FirstOrDefault();
                if (saved == null)
                {
                    saved = new UserEntity { Name = string.Empty };
                    db.Users.Add(saved);
                    db.SaveChanges();
                }

                // map persisted values into runtime User instance
                if (!string.IsNullOrWhiteSpace(saved.Name)) user.Name = saved.Name;
                user.LastTopic = saved.LastTopic ?? string.Empty;
                user.FavoriteTopic = saved.FavoriteTopic ?? string.Empty;

                foreach (var m in saved.Memory ?? new System.Collections.Generic.List<string>())
                {
                    user.Remember(m);
                }

                foreach (var i in saved.Interests ?? new System.Collections.Generic.List<string>())
                {
                    user.RememberInterest(i);
                }
            }

            if (args is null || args.Length == 0 || args[0].ToLowerInvariant() == "-gui")
            {
                var app = new Application();
                var main = new MainWindow(user);
                app.Run(main);

                // save user state after GUI closes
                using (var db = new AppDbContext(options))
                {
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

                // save user state after console run
                using (var db = new AppDbContext(options))
                {
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

                return;
            }

            var guiApp = new Application();
            guiApp.Run(new MainWindow(user));
>>>>>>> 431d709 (Add EF Core persistence, entities, README and persistence wiring)
        }
    }
}

