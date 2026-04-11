namespace CyberSecurityChatbot
{
    internal class User
    {
        public string Name { get; set; } = string.Empty;

        public bool HasName => !string.IsNullOrWhiteSpace(Name);
    }
}
