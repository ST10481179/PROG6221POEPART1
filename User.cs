using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityChatbot
{
    internal class User
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Memory { get; } = new();

        public List<string> Interests { get; } = new();

        public Dictionary<string, int> TopicResponseIndexes { get; } = new();

        public string LastTopic { get; set; } = string.Empty;

        public string FavoriteTopic { get; set; } = string.Empty;

        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        public bool HasInterests => Interests.Count > 0;

        public void Remember(string note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return;
            }

            var trimmed = note.Trim();
            // Avoid duplicates (case-insensitive)
            if (!Memory.Any(m => string.Equals(m, trimmed, System.StringComparison.OrdinalIgnoreCase)))
            {
                // Store a more readable memory (capitalize first letter)
                var stored = char.ToUpperInvariant(trimmed[0]) + (trimmed.Length > 1 ? trimmed.Substring(1) : string.Empty);
                Memory.Add(stored);
                try
                {
                    CyberSecurityChatbot.Data.Persistence.SaveUser(this);
                }
                catch
                {
                    // Fail silently if persistence is not available
                }
            }
        }

        public void RememberInterest(string interest)
        {
            if (string.IsNullOrWhiteSpace(interest))
            {
                return;
            }

            var trimmed = interest.Trim();
            var canonical = trimmed.ToLowerInvariant();
            if (!Interests.Any(existing => string.Equals(existing, canonical, System.StringComparison.OrdinalIgnoreCase)))
            {
                Interests.Add(trimmed);
            }

            FavoriteTopic = trimmed;

            Remember($"Interested in {trimmed}");
            try
            {
                CyberSecurityChatbot.Data.Persistence.SaveUser(this);
            }
            catch
            {
                // ignore persistence errors
            }
        }

        public string GetMemorySummary()
        {
            var memSummary = Memory.Count == 0
                ? "I don't have any details to remember yet."
                : "I remember: " + string.Join("; ", Memory);

            if (!string.IsNullOrWhiteSpace(FavoriteTopic))
            {
                return $"Favorite topic: {FavoriteTopic}. " + memSummary;
            }

            return memSummary;
        }

        public System.Collections.Generic.IEnumerable<string> GetMemoryList()
        {
            return Memory;
        }

        public System.Collections.Generic.IEnumerable<string> GetInterestsList()
        {
            return Interests;
        }

        internal void RestoreState(System.Collections.Generic.IEnumerable<string> memory, System.Collections.Generic.IEnumerable<string> interests, string favoriteTopic, string lastTopic)
        {
            Memory.Clear();
            Interests.Clear();

            foreach (var note in memory ?? System.Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(note) && !Memory.Contains(note))
                {
                    Memory.Add(note);
                }
            }

            foreach (var interest in interests ?? System.Array.Empty<string>())
            {
                if (!string.IsNullOrWhiteSpace(interest) && !Interests.Contains(interest))
                {
                    Interests.Add(interest);
                }
            }

            FavoriteTopic = favoriteTopic ?? string.Empty;
            LastTopic = lastTopic ?? string.Empty;
        }
    }
}
