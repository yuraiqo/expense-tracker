using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        [ObservableProperty] private string? username;
        [ObservableProperty] private string? password;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Both fields are required.");
                return;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (user == null)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }

            bool passwordMatch = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
            if (!passwordMatch)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }

            _navigationService.CurrentViewModel = new HomeViewModel(_navigationService, _dbContext);
        }
    }
}
