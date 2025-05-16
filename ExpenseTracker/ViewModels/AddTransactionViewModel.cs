using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExpenseTracker.ViewModels
{
    public partial class AddTransactionViewModel : ViewModelBase
    {
        // ----------------------------
        // Dependencies
        // ----------------------------

        private readonly AppDbContext _dbContext;
        private readonly User _user;
        private readonly Window _window;

        // ----------------------------
        // Callbacks
        // ----------------------------

        // Action to invoke after a transaction is saved (e.g., refresh transaction list)
        public Action OnTransactionSaved { get; set; }

        // ----------------------------
        // Constructor
        // ----------------------------

        public AddTransactionViewModel(AppDbContext db, User user, Window window)
        {
            _dbContext = db;
            _user = user;
            _window = window;

            // Load user's categories into collection for dropdown binding
            Categories = new ObservableCollection<Category>(
                _dbContext.Categories
                .Where(c => c.UserId == _user.Id)
                .ToList());
        }

        // ----------------------------
        // Properties (bound to UI)
        // ----------------------------

        public ObservableCollection<Category> Categories { get; }

        [ObservableProperty]
        private Category selectedCategory;

        [ObservableProperty]
        private decimal amount;

        // ----------------------------
        // Commands
        // ----------------------------

        // Saves a new transaction to the database
        [RelayCommand]
        private async Task SaveTransaction()
        {
            // Validate input
            if (SelectedCategory == null || Amount <= 0)
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            // Create transaction object
            var transaction = new Transaction
            {
                CategoryId = SelectedCategory.Id,
                UserId = _user.Id,
                Date = DateTime.UtcNow,
                Amount = Amount
            };

            // Update user balance based on transaction type
            if (SelectedCategory.Type == "Income")
            {
                _user.Balance += Amount;
            }
            else
            {
                _user.Balance -= Amount;
            }

            // Save transaction and updated user state
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            // Trigger callback to refresh parent view (if provided)
            OnTransactionSaved?.Invoke();

            // Close the dialog window
            _window.Close();
        }
    }
}
