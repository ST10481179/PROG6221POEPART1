using System.Collections.Generic;

namespace CyberSecurityChatbot.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> Memory { get; set; } = new();

        public List<string> Interests { get; set; } = new();

        public string LastTopic { get; set; } = string.Empty;

        public string FavoriteTopic { get; set; } = string.Empty;
    }
}
