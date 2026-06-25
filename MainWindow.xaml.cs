using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CyberSecurityChatbot
{
    internal partial class MainWindow : Window
    {
        private readonly User _user;
        private readonly TaskManager _taskManager;
        private readonly ActivityLogger _activityLogger;
        private readonly QuizManager _quizManager;
        private bool _awaitingName;
        private bool _awaitingReminder;
        private int _pendingTaskId;
        private bool _quizMode;

        private const string AsciiArt =
            "   ____ _       _                 ____        _   \r\n" +
            "  / ___| | ___ | |__   ___  _ __ / ___|  ___ | |_ \r\n" +
            " | |   | |/ _ \\| '_ \\ / _ \\| '__| |  _  / _ \\| __|\r\n" +
            " | |___| | (_) | |_) | (_) | |  | |_| || (_) | |_ \r\n" +
            "  \\____|_|\\___/|_.__/ \\___/|_|   \\____| \\___/ \\__|";

        public MainWindow(User user)
        {
            _user = user;
            _activityLogger = new ActivityLogger();
            _taskManager = new TaskManager(_activityLogger);
            _quizManager = new QuizManager();
            _awaitingName = true;
            _awaitingReminder = false;
            _pendingTaskId = 0;
            _quizMode = false;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            UpdateInputHint();
            InitializeHistory();
            RefreshTaskList();
            UpdateLogView();
            RefreshQuizPanel();
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

        private void RefreshTaskList()
        {
            TaskDataGrid.ItemsSource = null;
            TaskDataGrid.ItemsSource = _taskManager.GetAllTasks();
        }

        private void RefreshQuizPanel()
        {
            if (_quizManager.IsFinished())
            {
                QuizQuestionText.Text = $"Quiz complete. Score: {_quizManager.GetFinalScore()} / {_quizManager.TotalQuestions}. {_quizManager.GetFinalMessage()}";
                OptionARadio.Visibility = Visibility.Collapsed;
                OptionBRadio.Visibility = Visibility.Collapsed;
                OptionCRadio.Visibility = Visibility.Collapsed;
                OptionDRadio.Visibility = Visibility.Collapsed;
                SubmitAnswerButton.Visibility = Visibility.Collapsed;
                NextQuestionButton.Visibility = Visibility.Visible;
                NextQuestionButton.Content = "Restart Quiz";
                QuizFeedbackText.Text = string.Empty;
                QuizScoreText.Text = string.Empty;
                return;
            }

            var question = _quizManager.GetCurrentQuestion();
            QuizQuestionText.Text = question.Question;
            OptionARadio.Visibility = Visibility.Visible;
            OptionBRadio.Visibility = Visibility.Visible;
            OptionARadio.IsChecked = false;
            OptionBRadio.IsChecked = false;
            OptionCRadio.IsChecked = false;
            OptionDRadio.IsChecked = false;
            OptionARadio.Content = $"A) {question.Options[0]}";
            OptionBRadio.Content = $"B) {question.Options[1]}";
            if (question.IsTrueFalse)
            {
                OptionCRadio.Visibility = Visibility.Collapsed;
                OptionDRadio.Visibility = Visibility.Collapsed;
            }
            else
            {
                OptionCRadio.Visibility = Visibility.Visible;
                OptionDRadio.Visibility = Visibility.Visible;
                OptionCRadio.Content = question.Options.Count > 2 ? $"C) {question.Options[2]}" : "C)";
                OptionDRadio.Content = question.Options.Count > 3 ? $"D) {question.Options[3]}" : "D)";
            }

            SubmitAnswerButton.Visibility = Visibility.Visible;
            SubmitAnswerButton.IsEnabled = true;
            NextQuestionButton.Visibility = Visibility.Collapsed;
            QuizFeedbackText.Text = string.Empty;
            QuizScoreText.Text = $"Score: {_quizManager.Score} / {_quizManager.TotalQuestions}";
        }

        private void UpdateLogView()
        {
            LogTextBox.Text = _activityLogger.GetRecentLog(10);
            ShowMoreLogButton.IsEnabled = _activityLogger.GetCount() > 10;
        }

        private string? ProcessInput(string text)
        {
            var normalized = text.ToLowerInvariant();

            if (_quizMode)
            {
                if (normalized.Contains("exit quiz") || normalized.Contains("cancel quiz"))
                {
                    _quizMode = false;
                    RefreshQuizPanel();
                    return "Quiz ended. You can continue with other questions.";
                }

                if (normalized.Contains("show more"))
                {
                    LogTextBox.Text = _activityLogger.GetFullLog();
                    return "Here is the full activity history.";
                }

                return HandleQuizAnswer(text);
            }

            if (normalized.Contains("show activity log") || normalized.Contains("what have you done") || normalized.Contains("show log") || normalized.Contains("recent actions") || normalized.Contains("what did you do"))
            {
                MainTabControl.SelectedIndex = 3;
                UpdateLogView();
                return "Here\'s a summary of recent actions:\r\n" + LogTextBox.Text;
            }

            if (normalized.Contains("start quiz") || normalized.Contains("take quiz") || normalized.Contains("test my knowledge") || normalized.Contains("quiz me") || normalized.Contains("play the game"))
            {
                StartQuiz();
                return "Starting the cybersecurity quiz now.";
            }

            if (_awaitingReminder || normalized.Contains("remind me") || normalized.Contains("set a reminder") || normalized.Contains("reminder") || normalized.Contains("don\'t forget"))
            {
                return HandleReminderIntent(text);
            }

            if (IsTaskAddIntent(normalized))
            {
                return HandleTaskAddIntent(text);
            }

            return null;
        }

        private bool IsTaskAddIntent(string normalized)
        {
            return (normalized.Contains("add task") || normalized.Contains("add a task") || normalized.Contains("create task") || normalized.Contains("enable") || normalized.Contains("set up")) && normalized.Contains("task") || normalized.Contains("enable two-factor") || normalized.Contains("enable 2fa");
        }

        private string HandleTaskAddIntent(string text)
        {
            var normalized = text.ToLowerInvariant();
            var title = ExtractTaskTitle(text, normalized);
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "New cybersecurity task";
            }

            var description = title.EndsWith(".") ? title : title + ".";
            var task = _taskManager.AddTask(title, description, string.Empty);
            _pendingTaskId = task.Id;
            _awaitingReminder = true;
            RefreshTaskList();
            MainTabControl.SelectedIndex = 1;
            return $"Task added: '{title}'. Would you like to set a reminder for this task?";
        }

        private string HandleReminderIntent(string text)
        {
            if (!_awaitingReminder || _pendingTaskId == 0)
            {
                return "What task would you like me to remind you about?";
            }

            var reminder = ParseReminderText(text);
            if (string.IsNullOrWhiteSpace(reminder))
            {
                return "Please tell me when to remind you, for example 'Remind me in 3 days' or 'Tomorrow'.";
            }

            _taskManager.SetReminder(_pendingTaskId, reminder);
            _pendingTaskId = 0;
            _awaitingReminder = false;
            RefreshTaskList();
            return $"Got it! I\'ll remind you on {reminder}.";
        }

        private void StartQuiz()
        {
            _quizMode = true;
            _quizManager.ResetQuiz();
            _activityLogger.Log("Quiz started");
            MainTabControl.SelectedIndex = 2;
            RefreshQuizPanel();
        }

        private string HandleQuizAnswer(string text)
        {
            var selected = GetSelectedQuizAnswer();
            if (string.IsNullOrWhiteSpace(selected))
            {
                var candidate = text.Trim().ToUpperInvariant();
                if (candidate is "A" or "B" or "C" or "D")
                {
                    selected = candidate;
                }
                else
                {
                    return "Please select or type A, B, C, or D to answer.";
                }
            }

            var correct = _quizManager.SubmitAnswer(selected);
            var feedback = _quizManager.GetFeedback(correct);
            if (_quizManager.IsFinished())
            {
                _activityLogger.Log($"Quiz completed - score: {_quizManager.GetFinalScore()} out of {_quizManager.TotalQuestions}");
                RefreshQuizPanel();
                return feedback + " " + _quizManager.GetFinalMessage();
            }

            QuizFeedbackText.Text = feedback;
            QuizScoreText.Text = $"Score: {_quizManager.Score} / {_quizManager.TotalQuestions}";
            SubmitAnswerButton.IsEnabled = false;
            NextQuestionButton.Visibility = Visibility.Visible;
            return feedback;
        }

        private string ExtractTaskTitle(string original, string normalized)
        {
            var markers = new[] { "add task to", "add a task to", "create task to", "add task -", "add a task -", "create task -", "enable", "set up", "add task", "add a task", "create task" };
            foreach (var marker in markers)
            {
                if (normalized.Contains(marker))
                {
                    var index = normalized.IndexOf(marker, StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        var start = index + marker.Length;
                        if (start < original.Length)
                        {
                            var title = original.Substring(start).Trim();
                            title = title.Trim().TrimEnd('.', '!', '?');
                            return char.ToUpperInvariant(title.Length > 0 ? title[0] : 'N') + (title.Length > 1 ? title.Substring(1) : string.Empty);
                        }
                    }
                }
            }

            return original.Trim().TrimEnd('.', '!', '?');
        }

        private string ParseReminderText(string text)
        {
            var lower = text.ToLowerInvariant();
            if (lower.Contains("tomorrow"))
            {
                return System.DateTime.Today.AddDays(1).ToString("dd MMM yyyy");
            }
            var match = Regex.Match(lower, @"in\s+(\d+)\s+days");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var days))
            {
                return System.DateTime.Today.AddDays(days).ToString("dd MMM yyyy");
            }
            if (lower.Contains("in 1 day"))
            {
                return System.DateTime.Today.AddDays(1).ToString("dd MMM yyyy");
            }
            match = Regex.Match(lower, @"on\s+([A-Za-z0-9\s,-]+)");
            if (match.Success)
            {
                var candidate = match.Groups[1].Value.Trim();
                if (System.DateTime.TryParse(candidate, out var date))
                {
                    return date.ToString("dd MMM yyyy");
                }
            }
            return string.Empty;
        }

        private string GetSelectedQuizAnswer()
        {
            if (OptionARadio.IsChecked == true) return "A";
            if (OptionBRadio.IsChecked == true) return "B";
            if (OptionCRadio.IsChecked == true) return "C";
            if (OptionDRadio.IsChecked == true) return "D";
            return string.Empty;
        }

        private int GetSelectedTaskId()
        {
            if (TaskDataGrid.SelectedItem is UserTask selected)
            {
                return selected.Id;
            }

            return 0;
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

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var title = TaskTitleTextBox.Text.Trim();
            var description = TaskDescriptionTextBox.Text.Trim();
            var reminder = TaskReminderTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title before adding the task.", "Task Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var task = _taskManager.AddTask(title, description, reminder);
            if (!string.IsNullOrWhiteSpace(reminder))
            {
                _taskManager.SetReminder(task.Id, reminder);
            }

            TaskTitleTextBox.Clear();
            TaskDescriptionTextBox.Clear();
            TaskReminderTextBox.Clear();
            RefreshTaskList();
            UpdateLogView();
            MainTabControl.SelectedIndex = 1;
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var taskId = GetSelectedTaskId();
            if (taskId == 0) return;
            _taskManager.MarkAsComplete(taskId);
            RefreshTaskList();
            UpdateLogView();
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var taskId = GetSelectedTaskId();
            if (taskId == 0) return;
            _taskManager.DeleteTask(taskId);
            RefreshTaskList();
            UpdateLogView();
        }

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var response = HandleQuizAnswer(string.Empty);
            QuizFeedbackText.Text = response;
            if (!_quizManager.IsFinished())
            {
                SubmitAnswerButton.IsEnabled = false;
                NextQuestionButton.Visibility = Visibility.Visible;
            }
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_quizManager.IsFinished())
            {
                _quizManager.ResetQuiz();
                RefreshQuizPanel();
                return;
            }

            _quizManager.Advance();
            RefreshQuizPanel();
        }

        private void ShowMoreLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogTextBox.Text = _activityLogger.GetFullLog();
            ShowMoreLogButton.IsEnabled = false;
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
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (string.IsNullOrWhiteSpace(_user.Name))
                    {
                        HistoryTextBox.AppendText("Bot: Please enter your name before continuing.\r\n");
                        InputTextBox.Clear();
                        InputTextBox.Focus();
                        return;
                    }
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

            var customResponse = ProcessInput(text);
            if (customResponse != null)
            {
                HistoryTextBox.AppendText($"Bot: {customResponse}\r\n");
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
