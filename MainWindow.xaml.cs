using System.Threading.Tasks;
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
            Loaded += MainWindow_Loaded;
            UpdateInputHint();
            InitializeHistory();
            InputTextBox.Focus();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            AudioPlayer.PlayGreeting();
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
            MemoryListBox.Items.Clear();
            foreach (var m in _user.GetMemoryList())
            {
                MemoryListBox.Items.Add(m);
            }
        }

        private void ClearMemories_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all memories?", "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _user.GetMemoryList().ToList().Clear();
                // The above returns a copy; clear the underlying list instead
                // Access internal list via reflection-free approach: use a temporary list and clear by remembering an API on User
                // Since Memory is internal `List<string> Memory { get; }` we can clear via the public GetMemoryList only by casting
                if (_user.GetMemoryList() is System.Collections.Generic.List<string> memList)
                {
                    memList.Clear();
                }
                else
                {
                    // fallback: rebuild user memory via reflection
                    var memProp = typeof(User).GetProperty("Memory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    if (memProp != null)
                    {
                        if (memProp.GetValue(_user) is System.Collections.IList list)
                        {
                            list.Clear();
                        }
                    }
                }

                try
                {
                    CyberSecurityChatbot.Data.Persistence.SaveUser(_user);
                }
                catch { }

                UpdateStatusPanel();
                HistoryTextBox.AppendText("\r\nBot: Cleared memories.\r\n");
                HistoryTextBox.ScrollToEnd();
            }
        }

        private void ClearInterests_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all interests?", "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var interestsProp = typeof(User).GetProperty("Interests", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (interestsProp != null)
                {
                    if (interestsProp.GetValue(_user) is System.Collections.IList list)
                    {
                        list.Clear();
                    }
                }

                try
                {
                    CyberSecurityChatbot.Data.Persistence.SaveUser(_user);
                }
                catch { }

                UpdateStatusPanel();
                HistoryTextBox.AppendText("\r\nBot: Cleared interests.\r\n");
                HistoryTextBox.ScrollToEnd();
            }
        }

        private void RemoveMemory_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryListBox.SelectedItem == null) return;
            var selected = MemoryListBox.SelectedItem.ToString();
            var result = MessageBox.Show($"Remove memory: '{selected}'?", "Confirm Remove", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var memProp = typeof(User).GetProperty("Memory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (memProp != null)
                {
                    if (memProp.GetValue(_user) is System.Collections.IList list)
                    {
                        list.Remove(selected);
                    }
                }

                try { CyberSecurityChatbot.Data.Persistence.SaveUser(_user); } catch { }
                UpdateStatusPanel();
                HistoryTextBox.AppendText("\r\nBot: Memory removed.\r\n");
                HistoryTextBox.ScrollToEnd();
            }
        }

        private void EditMemory_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryListBox.SelectedItem == null) return;
            var selected = MemoryListBox.SelectedItem?.ToString() ?? string.Empty;
            var dlg = new InputDialog("Edit Memory", selected);
            if (dlg.ShowDialog() == true)
            {
                var newVal = dlg.ResponseText.Trim();
                if (!string.IsNullOrWhiteSpace(newVal))
                {
                    var memProp = typeof(User).GetProperty("Memory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    if (memProp != null)
                    {
                        if (memProp.GetValue(_user) is System.Collections.IList list)
                        {
                            var idx = list.IndexOf(selected);
                            if (idx >= 0)
                            {
                                list[idx] = newVal;
                            }
                        }
                    }

                    try { CyberSecurityChatbot.Data.Persistence.SaveUser(_user); } catch { }
                    UpdateStatusPanel();
                    HistoryTextBox.AppendText("\r\nBot: Memory edited.\r\n");
                    HistoryTextBox.ScrollToEnd();
                }
            }
        }

        private void AddMemory_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new InputDialog("Add Memory", "");
            if (dlg.ShowDialog() == true)
            {
                var newVal = dlg.ResponseText.Trim();
                if (!string.IsNullOrWhiteSpace(newVal))
                {
                    _user.Remember(newVal);
                    UpdateStatusPanel();
                    HistoryTextBox.AppendText("\r\nBot: Memory added.\r\n");
                    HistoryTextBox.ScrollToEnd();
                }
            }
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
