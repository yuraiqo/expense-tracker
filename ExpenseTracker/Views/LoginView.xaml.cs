using System.Windows;
using System.Windows.Controls;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
                await vm.LoginCommand.ExecuteAsync(null);
            }
        }
    }
}
