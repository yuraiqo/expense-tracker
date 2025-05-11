using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Services;
using System.Windows;

namespace ExpenseTracker.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private readonly AppDbContext _dbContext;

        public LoginViewModel(NavigationService ns, AppDbContext db)
        {
            _navigationService = ns;
            _dbContext = db;
        }

        [RelayCommand]
        private void NavigateRegister()
        {
            _navigationService.CurrentViewModel = new RegisterViewModel(_navigationService, _dbContext);
        }

        [RelayCommand]
        private void Login()
        {
            MessageBox.Show("Ahoj");
        }

        [RelayCommand]
        private void NavigateHome()
        {
            _navigationService.CurrentViewModel = new HomeViewModel(_navigationService, _dbContext);
        }
    }
}
