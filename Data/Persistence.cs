using System.Linq;
using CyberSecurityChatbot.Entities;

namespace CyberSecurityChatbot.Data
{
    internal static class Persistence
    {
        internal static void SaveUser(CyberSecurityChatbot.User user)
        {
            var factory = new AppDbContextFactory();
            using var db = factory.CreateDbContext(System.Array.Empty<string>());

            var existing = db.Users.FirstOrDefault();
            if (existing == null)
            {
                existing = new UserEntity();
                db.Users.Add(existing);
            }

            existing.Name = user.Name ?? string.Empty;
            existing.LastTopic = user.LastTopic ?? string.Empty;
            existing.FavoriteTopic = user.FavoriteTopic ?? string.Empty;
            existing.Memory = user.GetMemoryList().ToList();
            existing.Interests = user.GetInterestsList().ToList();

            db.SaveChanges();
        }
    }
}
