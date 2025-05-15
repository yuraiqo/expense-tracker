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
        private readonly AppDbContext _dbContext;
        private readonly User _user;
        private readonly Window _window;

        public Action OnTransactionSaved { get; set; } 

        public AddTransactionViewModel(AppDbContext db, User user, Window window)
        {
            _dbContext = db;
            _user = user;
            _window = window;

            Categories = new ObservableCollection<Category>(_dbContext.Categories.Where(c => c.UserId == _user.Id).ToList());
        }

        public ObservableCollection<Category> Categories { get; }


        [ObservableProperty] private Category selectedCategory;
        [ObservableProperty] private decimal amount;

        [RelayCommand]
        private async Task SaveTransaction()
        {
            if (SelectedCategory == null || Amount <= 0)
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            var transaction = new Transaction
            {
                CategoryId = SelectedCategory.Id,
                UserId = _user.Id,
                Date = DateTime.UtcNow,
                Amount = Amount
            };

            if (SelectedCategory.Type == "Income")
            {
                _user.Balance += Amount;
            } else
            {
                _user.Balance -= Amount;
            }

                _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            OnTransactionSaved?.Invoke();

            _window.Close();
        }
    }
}
