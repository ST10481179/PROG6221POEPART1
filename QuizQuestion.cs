using System.Collections.Generic;

namespace CyberSecurityChatbot
{
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public bool IsTrueFalse { get; set; }
    }
}
