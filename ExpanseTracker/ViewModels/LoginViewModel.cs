using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Views;
using System.Windows;
using System.Windows.Navigation;

namespace ExpenseTracker.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public RelayCommand LoginCommand => new RelayCommand(Login);

        private void Login()
        {
            MessageBox.Show("Ahoj");
        }
    }
}
