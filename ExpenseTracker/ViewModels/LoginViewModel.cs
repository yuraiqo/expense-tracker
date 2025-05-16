using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        // ----------------------------
        // Dependencies
        // ----------------------------

        private readonly NavigationService _navigationService;
        private readonly AppDbContext _dbContext;

        // ----------------------------
        // Constructor
        // ----------------------------

        public LoginViewModel(NavigationService ns, AppDbContext db)
        {
            _navigationService = ns;
            _dbContext = db;
        }

        // ----------------------------
        // Properties (bound to UI)
        // ----------------------------

        [ObservableProperty]
        private string? username;

        [ObservableProperty]
        private string? password;

        // ----------------------------
        // Commands
        // ----------------------------

        // Navigate to the registration screen
        [RelayCommand]
        private void NavigateRegister()
        {
            _navigationService.CurrentViewModel = new RegisterViewModel(_navigationService, _dbContext);
        }

        // Attempt to log in the user asynchronously
        [RelayCommand]
        private async Task LoginAsync()
        {
            // Validate that both username and password are provided
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Both fields are required.");
                return;
            }

            // Attempt to find user by username in the database
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (user == null)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }

            // Verify the entered password against the stored hash
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
            if (!passwordMatch)
            {
                MessageBox.Show("Invalid username or password.");
                return;
            }

            // Successful login: navigate to home screen passing the authenticated user
            _navigationService.CurrentViewModel = new HomeViewModel(_navigationService, _dbContext, user);
        }
    }
}
