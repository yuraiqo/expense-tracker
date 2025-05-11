using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Services;

namespace ExpenseTracker.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private readonly AppDbContext _dbContext;

        public RegisterViewModel(NavigationService ns, AppDbContext db)
        {
            _navigationService = ns;
            _dbContext = db;
        }

        [RelayCommand]
        private void NavigateLogin()
        {
            _navigationService.CurrentViewModel = new LoginViewModel(_navigationService, _dbContext);
        }

        [RelayCommand]
        private void NavigateHome()
        {
            _navigationService.CurrentViewModel = new HomeViewModel(_navigationService, _dbContext);
        }
    }
}
