using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityChatbot
{
    internal class ActivityLogger
    {
        private readonly ApplicationDbContext db = new();

        public ActivityLogger()
        {
            db.Database.EnsureCreated();
        }

        public void Log(string action)
        {
            var entry = new ActivityLogEntry
            {
                Description = action,
                CreatedAt = System.DateTime.Now.ToString("HH:mm")
            };
            db.Logs.Add(entry);
            db.SaveChanges();
        }

        public string GetRecentLog(int count = 10)
        {
            var entries = db.Logs.OrderByDescending(l => l.Id).Take(count).ToList();
            if (entries.Count == 0)
            {
                return "No recent actions logged yet.";
            }
            entries.Reverse();
            return FormatEntries(entries);
        }

        public string GetFullLog()
        {
            var entries = db.Logs.OrderBy(l => l.Id).ToList();
            if (entries.Count == 0)
            {
                return "No actions logged yet.";
            }
            return FormatEntries(entries);
        }

        public int GetCount()
        {
            return db.Logs.Count();
        }

        private static string FormatEntries(List<ActivityLogEntry> entries)
        {
            var lines = new List<string>();
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                lines.Add($"{i + 1}. {entry.Description} ({entry.CreatedAt})");
            }
            return string.Join("\r\n", lines);
        }
    }
}
