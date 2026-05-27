using System.Windows;
using System.Windows.Input;

namespace CyberSecurityChatbot
{
    internal partial class MainWindow : Window
    {
        private readonly User _user;
        private bool _awaitingName = true;

        public MainWindow(User user)
        {
            _user = user;
            InitializeComponent();
            AudioPlayer.PlayGreeting();
            UpdateInputHint();
            HistoryTextBox.Text = "Welcome to the Cybersecurity Awareness Bot!\r\n" +
                "Please enter your name so I can personalize your experience.\r\n";
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

        private void SendMessage()
        {
            var text = InputTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var senderName = _awaitingName ? "You" : (string.IsNullOrWhiteSpace(_user.Name) ? "You" : _user.Name);
            HistoryTextBox.AppendText($"\r\n{senderName}: {text}\r\n");

            if (_awaitingName)
            {
                _user.Name = text;
                _awaitingName = false;
                UpdateInputHint();
                HistoryTextBox.AppendText($"Bot: Nice to meet you, {_user.Name}! I can answer questions about passwords, phishing, malware, VPNs, privacy, and safe browsing. You can also ask for another tip or tell me what you’re interested in.\r\n");
                InputTextBox.Clear();
                HistoryTextBox.ScrollToEnd();
                return;
            }

            HistoryTextBox.AppendText($"Bot: {ChatLogic.GetResponse(_user, text)}\r\n");
            InputTextBox.Clear();
            HistoryTextBox.ScrollToEnd();
        }

        private void UpdateInputHint()
        {
          
        }
    }
}