using System.Windows;

namespace CyberSecurityChatbot
{
    public partial class InputDialog : Window
    {
        public string ResponseText => ResponseBox.Text ?? string.Empty;

        public InputDialog(string prompt, string initial = "")
        {
            InitializeComponent();
            PromptText.Text = prompt;
            ResponseBox.Text = initial;
            ResponseBox.Focus();
            ResponseBox.SelectAll();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
