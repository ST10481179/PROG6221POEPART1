namespace CyberSecurityChatbot
{
    public class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }
}
