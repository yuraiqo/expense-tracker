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

        [ObservableProperty] private string? username;
        [ObservableProperty] private string? email;
        [ObservableProperty] private string? password;
        [ObservableProperty] private string? confirmPassword;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            // simple validation
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            if (!new EmailAddressAttribute().IsValid(Email))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            // check if user exists
            bool exists = await _dbContext.Users.AnyAsync(u => u.Email == Email || u.Username == username);
            if (exists)
            {
                MessageBox.Show("A user with this email or username already exists.");
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var newUser = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            _navigationService.CurrentViewModel = new HomeViewModel(_navigationService, _dbContext, newUser);
        }
    }
}
