using System.Collections.Generic;

namespace CyberSecurityChatbot
{
    internal class QuizManager
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;

        public QuizManager()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectAnswer = "C",
                    Explanation = "Correct! Reporting phishing emails helps prevent scams.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "A strong password should be at least 12 characters long.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "A",
                    Explanation = "True. Longer passwords are harder to guess.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "Which practice improves password safety?",
                    Options = new List<string> { "Use the same password for every site", "Write passwords on paper and leave them open", "Use a password manager", "Share passwords with friends" },
                    CorrectAnswer = "C",
                    Explanation = "A password manager helps you store unique secure passwords.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "Using public Wi-Fi without a VPN is safe for sensitive banking." ,
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "B",
                    Explanation = "False. Public Wi-Fi can expose your data without encryption.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What is the best action when a website does not use HTTPS?",
                    Options = new List<string> { "Enter your password anyway", "Leave the site and avoid sharing data", "Refresh the page", "Disable your browser" },
                    CorrectAnswer = "B",
                    Explanation = "Sites without HTTPS do not encrypt your traffic, so avoid sharing data.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "Social engineering attacks rely on manipulating people rather than technical vulnerabilities.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "A",
                    Explanation = "True. These attacks trick people into sharing information or access.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What does two-factor authentication require?",
                    Options = new List<string> { "Only a password", "A password and a second verification method", "Only a fingerprint", "Only a phone number" },
                    CorrectAnswer = "B",
                    Explanation = "Two-factor authentication adds a second layer beyond passwords.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "Which is a safer way to back up your data?",
                    Options = new List<string> { "Never back up files", "Back up only to public folders", "Use a secure cloud or offline backup", "Delete old backups" },
                    CorrectAnswer = "C",
                    Explanation = "Secure backups help you recover from malware and ransomware.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "If a stranger asks for your account details over the phone, you should give them the information.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "B",
                    Explanation = "False. Legitimate services will not ask for passwords over the phone.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "Which of these protects your privacy on social networks?",
                    Options = new List<string> { "Make all posts public", "Share passwords with friends", "Review and tighten privacy settings", "Use easy-to-guess profile information" },
                    CorrectAnswer = "C",
                    Explanation = "Reviewing privacy settings helps control who sees your information.",
                    IsTrueFalse = false
                }
            };
            ResetQuiz();
        }

        public QuizQuestion GetCurrentQuestion()
        {
            return _questions[_currentIndex];
        }

        public bool SubmitAnswer(string answer)
        {
            if (_currentIndex >= _questions.Count) return false;
            var correct = string.Equals(answer.Trim(), _questions[_currentIndex].CorrectAnswer.Trim(), System.StringComparison.OrdinalIgnoreCase);
            if (correct) _score++;
            return correct;
        }

        public string GetFeedback(bool correct)
        {
            var explanation = _questions[_currentIndex].Explanation;
            return correct ? $"Correct! {explanation}" : $"Incorrect. {explanation}";
        }

        public bool IsFinished()
        {
            return _currentIndex >= _questions.Count;
        }

        public int GetFinalScore()
        {
            return _score;
        }

        public string GetFinalMessage()
        {
            return _score >= _questions.Count * 0.8
                ? "Great job! You know your cybersecurity basics."
                : "Keep learning. Review the topics and try again.";
        }

        public void Advance()
        {
            if (_currentIndex < _questions.Count) _currentIndex++;
        }

        public void ResetQuiz()
        {
            _currentIndex = 0;
            _score = 0;
        }

        public int TotalQuestions => _questions.Count;
        public int CurrentIndex => _currentIndex;
        public int Score => _score;
    }
}
