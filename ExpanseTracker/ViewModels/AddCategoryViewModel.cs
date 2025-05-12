using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ViewModels
{
    public partial class AddCategoryViewModel : ViewModelBase
    {
        private readonly AppDbContext _dbContext;
        private readonly User _user;
        private readonly Window _window;

        public AddCategoryViewModel(AppDbContext db, User user, Window window)
        {
            _dbContext = db;
            _user = user;
            _window = window;
        }

        [ObservableProperty] private string name;
        [ObservableProperty] private string selectedColor;
        [ObservableProperty] private bool isExpense;
        [ObservableProperty] private bool isIncome;

        [RelayCommand]
        private async Task SaveCategory()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(SelectedColor) || (IsExpense == false && IsIncome == false))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            var exists = await _dbContext.Categories.AnyAsync(c => c.UserId == _user.Id && c.Name.ToLower() == Name.Trim().ToLower());
            if (exists)
            {
                MessageBox.Show("A category with this name already exists.");
                return;
            }

            var category = new Category
            {
                Name = Name.Trim(),
                UserId = _user.Id,
                Type = IsExpense ? "Expense" : "Income",
                Color = SelectedColor
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            _window.Close();
        }

    }
}
