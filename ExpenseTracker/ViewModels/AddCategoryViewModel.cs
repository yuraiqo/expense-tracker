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
        // ----------------------------
        // Dependencies
        // ----------------------------
        private readonly AppDbContext _dbContext;
        private readonly User _user;
        private readonly Window _window;

        // Constructor
        public AddCategoryViewModel(AppDbContext db, User user, Window window)
        {
            _dbContext = db;
            _user = user;
            _window = window;
        }

        public Action OnCategoryAdded { get; set; }


        // ----------------------------
        // Properties (bound to UI)
        // ----------------------------

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string selectedColor;

        [ObservableProperty]
        private bool isExpense;

        [ObservableProperty]
        private bool isIncome;

        // ----------------------------
        // Commands
        // ----------------------------

        // Saves a new category to the database
        [RelayCommand]
        private async Task SaveCategory()
        {
            // Validation: ensure all required fields are filled
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(SelectedColor) || (IsExpense == false && IsIncome == false))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            // Check for existing category with the same name for this user
            var exists = await _dbContext.Categories.AnyAsync(c =>
                c.UserId == _user.Id &&
                c.Name.ToLower() == Name.Trim().ToLower());

            if (exists)
            {
                MessageBox.Show("A category with this name already exists.");
                return;
            }

            // Create and add new category
            var category = new Category
            {
                Name = Name.Trim(),
                UserId = _user.Id,
                Type = IsExpense ? "Expense" : "Income",
                Color = SelectedColor
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            OnCategoryAdded?.Invoke();

            // Close the window after successful save
            _window.Close();
        }
    }
}
