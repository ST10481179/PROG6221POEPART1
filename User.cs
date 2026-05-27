using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityChatbot
{
    internal class User
    {
        public string Name { get; set; } = string.Empty;

        public List<string> Memory { get; } = new();

        public List<string> Interests { get; } = new();

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
            if (!Memory.Contains(trimmed))
            {
                Memory.Add(trimmed);
            }
        }

        public void RememberInterest(string interest)
        {
            if (string.IsNullOrWhiteSpace(interest))
            {
                return;
            }

            var trimmed = interest.Trim();
            if (!Interests.Contains(trimmed))
            {
                Interests.Add(trimmed);
            }

            FavoriteTopic = trimmed;

            Remember($"Interested in {trimmed}");
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
    }
}
