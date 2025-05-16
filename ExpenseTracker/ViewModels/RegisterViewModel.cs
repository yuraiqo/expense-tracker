using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        // ----------------------------
        // Dependencies
        // ----------------------------

        private readonly NavigationService _navigationService;
        private readonly AppDbContext _dbContext;

        // ----------------------------
        // Constructor
        // ----------------------------

        public RegisterViewModel(NavigationService ns, AppDbContext db)
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
        private string? email;

        [ObservableProperty]
        private string? password;

        [ObservableProperty]
        private string? confirmPassword;

        // ----------------------------
        // Commands
        // ----------------------------

        // Navigate to the login screen
        [RelayCommand]
        private void NavigateLogin()
        {
            _navigationService.CurrentViewModel = new LoginViewModel(_navigationService, _dbContext);
        }

        /// <summary>
        /// Register a new user asynchronously after validating inputs.
        /// Hashes the password and adds the user to the database.
        /// Navigates to the home view on success.
        /// </summary>
        [RelayCommand]
        private async Task RegisterAsync()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            // Validate email format
            if (!new EmailAddressAttribute().IsValid(Email))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            // Validate password confirmation
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // Check if a user with same email or username already exists
            bool exists = await _dbContext.Users.AnyAsync(u => u.Email == Email || u.Username == username);
            if (exists)
            {
                MessageBox.Show("A user with this email or username already exists.");
                return;
            }

            // Hash the password before saving
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            // Create new user entity
            var newUser = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            // Add user to database and save changes
            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Navigate to home view with the newly registered user
            _navigationService.CurrentViewModel = new HomeViewModel(_navigationService, _dbContext, newUser);
        }
    }
}
