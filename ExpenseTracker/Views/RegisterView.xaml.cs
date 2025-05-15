using ExpenseTracker.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ExpenseTracker.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }
        
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.Password = PasswordBox.Password;
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
                await vm.RegisterCommand.ExecuteAsync(null);
            }
        }
    }
}
