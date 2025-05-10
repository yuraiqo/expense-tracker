using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Services;

namespace ExpenseTracker.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        public RegisterViewModel(NavigationService ns)
        {
            _navigationService = ns;
        }

        [RelayCommand]
        private void NavigateLogin()
        {
            _navigationService.CurrentViewModel = new LoginViewModel(_navigationService);
        }


    }
}
