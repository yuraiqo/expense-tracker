using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.Views;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private readonly AppDbContext _dbContext;

        private readonly User _user;

        public HomeViewModel(NavigationService ns, AppDbContext db, User user)
        {
            _navigationService = ns;
            _dbContext = db;
            _user = user;

            LoadTransactionsAsync();
        }

        public ObservableCollection<Transaction> Transactions { get; private set; }
        public string Username => _user.Username;

        private async Task LoadTransactionsAsync()
        {
            if (Transactions == null)
            {
                Transactions = new ObservableCollection<Transaction>();
            }
            else
            {
                Transactions.Clear();
            }

            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var items = await _dbContext.Transactions
                .Where(t => t.UserId == _user.Id)
                .OrderByDescending(t => t.Date)
                .Include(t => t.Category)
                .ToListAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in items)
                {
                    Transactions.Add(item);
                }
            });

            CalculateAmountSpentAndIncome(items);
        }

        [ObservableProperty] private decimal spentThisMonth;
        [ObservableProperty] private decimal incomeThisMonth;

        private void CalculateAmountSpentAndIncome(List<Transaction> transactions)
        {
            SpentThisMonth = transactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);

            IncomeThisMonth = transactions
                .Where(t => t.Category.Type == "Income")
                .Sum(t => t.Amount);
        }

        [RelayCommand]
        private void OpenAddCategory()
        {
            var window = new AddCategoryView(_dbContext, _user);
            window.ShowDialog();
        }

        [RelayCommand]
        private void OpenAddTransaction()
        {
            var window = new AddTransactionView(_dbContext, _user);

            if (window.DataContext is AddTransactionViewModel viewmodel)
            {
                viewmodel.OnTransactionSaved = async () => await LoadTransactionsAsync();
            }

            window.ShowDialog();
        }

        [RelayCommand]
        private void Logout()
        {
            _navigationService.CurrentViewModel = new LoginViewModel(_navigationService, _dbContext);
        }
    }
}
