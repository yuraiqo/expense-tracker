using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Services;
using System.Windows;

namespace ExpenseTracker.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        public LoginViewModel(NavigationService ns)
        {
            _navigationService = ns;
        }

        [RelayCommand]
        private void NavigateRegister()
        {
            _navigationService.CurrentViewModel = new RegisterViewModel(_navigationService);
        }

        [RelayCommand]
        private void Login()
        {
            MessageBox.Show("Ahoj");
        }
    }
}
