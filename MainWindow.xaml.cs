using System.Windows;
using System.Windows.Input;

namespace CyberSecurityChatbot
{
    internal partial class MainWindow : Window
    {
        private readonly User _user;
        private bool _awaitingName;

        private const string AsciiArt =
            "   ____ _       _                 ____        _   \r\n" +
            "  / ___| | ___ | |__   ___  _ __ / ___|  ___ | |_ \r\n" +
            " | |   | |/ _ \\| '_ \\ / _ \\| '__| |  _  / _ \\| __|\r\n" +
            " | |___| | (_) | |_) | (_) | |  | |_| || (_) | |_ \r\n" +
            "  \\____|_|\\___/|_.__/ \\___/|_|   \\____| \\___/ \\__|";

        public MainWindow(User user)
        {
            _user = user;
            // Always ask for the user's name on startup, even if previously saved.
            _awaitingName = true;
            InitializeComponent();
            AudioPlayer.PlayGreeting();
            UpdateInputHint();
            InitializeHistory();
            InputTextBox.Focus();
        }

        private void InitializeHistory()
        {
            HistoryTextBox.Text = AsciiArt + "\r\n\r\n";

            if (_awaitingName)
            {
                HistoryTextBox.AppendText("Welcome to the Cybersecurity Awareness Bot!\r\n");
                if (!string.IsNullOrWhiteSpace(_user.Name))
                {
                    HistoryTextBox.AppendText($"I have a saved name: {_user.Name}. Press Send (leave blank) to keep it, or type a new name.\r\n");
                }
                else
                {
                    HistoryTextBox.AppendText("Please enter your name so I can personalise answers and remember your interests.\r\n");
                }
                HistoryTextBox.AppendText("Ask about passwords, phishing, malware, VPNs, privacy, or safe browsing." + "\r\n");
            }
            else
            {
                HistoryTextBox.AppendText($"Welcome back, {_user.Name}!\r\n");
                HistoryTextBox.AppendText(_user.GetMemorySummary() + "\r\n");
                HistoryTextBox.AppendText("Ask another cybersecurity question or say 'help' for examples.\r\n");
            }

            UpdateStatusPanel();
            HistoryTextBox.ScrollToEnd();
        }

        private void UpdateStatusPanel()
        {
            UserNameText.Text = string.IsNullOrWhiteSpace(_user.Name) ? "Not set" : _user.Name;
            LastTopicText.Text = string.IsNullOrWhiteSpace(_user.LastTopic) ? "No topic yet" : _user.LastTopic;
            InterestsText.Text = _user.HasInterests ? string.Join(", ", _user.GetInterestsList()) : "No interests yet";
            MemorySummaryTextBox.Text = _user.GetMemorySummary();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryTextBox.AppendText("\r\nBot: Ask me about passwords, phishing, malware, VPNs, privacy, or safe browsing.\r\n");
            HistoryTextBox.AppendText("Try: 'Tell me about password safety', 'Give me another tip', or 'I'm interested in privacy'.\r\n");
            HistoryTextBox.ScrollToEnd();
        }

        private void SendMessage()
        {
            var text = InputTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            HistoryTextBox.AppendText($"\r\nYou: {text}\r\n");

            if (_awaitingName)
            {
                // If user enters a name, use it. If empty and a saved name exists, keep saved name.
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (string.IsNullOrWhiteSpace(_user.Name))
                    {
                        // Force the user to provide a name if none is saved.
                        HistoryTextBox.AppendText("Bot: Please enter your name before continuing.\r\n");
                        InputTextBox.Clear();
                        InputTextBox.Focus();
                        return;
                    }
                    // keep existing saved name
                }
                else
                {
                    _user.Name = text;
                }

                _awaitingName = false;
                UpdateInputHint();
                HistoryTextBox.AppendText($"Bot: Nice to meet you, {_user.Name}! I can answer questions about passwords, phishing, malware, VPNs, privacy, and safe browsing.\r\n");
                InputTextBox.Clear();
                UpdateStatusPanel();
                HistoryTextBox.ScrollToEnd();
                return;
            }

            var response = ChatLogic.GetResponse(_user, text);
            HistoryTextBox.AppendText($"Bot: {response}\r\n");
            InputTextBox.Clear();
            UpdateStatusPanel();
            HistoryTextBox.ScrollToEnd();
        }

        private void UpdateInputHint()
        {
            if (_awaitingName)
            {
                InputTextBox.ToolTip = "Enter your name and press Send";
            }
            else
            {
                InputTextBox.ToolTip = "Type a cybersecurity question and press Send";
            }
        }
    }
}
