using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace ExpenseTracker.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        // ----------------------------
        // Dependencies
        // ----------------------------
        private readonly NavigationService _navigationService;
        private readonly AppDbContext _dbContext;
        private readonly User _user;

        // ----------------------------
        // Constructor & Initialization
        // ----------------------------
        public HomeViewModel(NavigationService ns, AppDbContext db, User user)
        {
            _navigationService = ns;
            _dbContext = db;
            _user = user;

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadTransactionsAsync();
            await LoadCategoriesAsync();
        }

        // ----------------------------
        // Properties & Observable Properties
        // ----------------------------
        public ObservableCollection<Transaction> Transactions { get; private set; }

        public string Username => _user.Username;

        [ObservableProperty]
        private decimal spentOnLoaded;

        [ObservableProperty]
        private decimal incomeOnLoaded;

        [ObservableProperty]
        private decimal balance;

        public ObservableCollection<string> CategoryFilterOptions { get; } = new();

        [ObservableProperty]
        private string selectedCategoryFilter = "All";

        public ObservableCollection<string> TimeFilterOptions { get; } = new()
        {
            "All",
            "Last week",
            "Last month",
            "Last year"
        };

        [ObservableProperty]
        private string selectedTimeFilter = "All";

        // ----------------------------
        // Private Fields
        // ----------------------------
        private bool isExportingData;

        // ----------------------------
        // Data Loading Methods
        // ----------------------------

        /// <summary>
        /// Loads transactions applying the selected filters.
        /// </summary>
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

            var query = _dbContext.Transactions
                .Where(t => t.UserId == _user.Id)
                .Include(t => t.Category)
                .AsQueryable();

            // Apply category filter
            if (SelectedCategoryFilter != "All")
            {
                query = query.Where(t => t.Category.Name == SelectedCategoryFilter);
            }

            // Apply time filter
            DateTime filterDate = DateTime.MinValue;
            DateTime utcNowDate = DateTime.UtcNow.Date;

            switch (SelectedTimeFilter)
            {
                case "Last week":
                    filterDate = utcNowDate.AddDays(-7);
                    break;
                case "Last month":
                    filterDate = utcNowDate.AddDays(-30);
                    break;
                case "Last year":
                    filterDate = utcNowDate.AddDays(-365);
                    break;
                case "All":
                default:
                    break;
            }

            if (filterDate > DateTime.MinValue)
            {
                query = query.Where(t => t.Date >= filterDate);
            }

            var items = await query.OrderByDescending(t => t.Date).ToListAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in items)
                {
                    Transactions.Add(item);
                }
            });

            CalculateAmountSpentAndIncome(items);

            // Update balance from DB
            var userFromDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == _user.Id);
            if (userFromDb != null)
            {
                Balance = userFromDb.Balance;
            }
        }

        /// <summary>
        /// Loads category filter options from the database.
        /// </summary>
        private async Task LoadCategoriesAsync()
        {
            var categories = await _dbContext.Categories
                .Where(c => c.UserId == _user.Id)
                .ToListAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (!CategoryFilterOptions.Contains("All"))
                {
                    CategoryFilterOptions.Add("All");
                }

                foreach (var category in categories)
                {
                    if (!CategoryFilterOptions.Contains(category.Name))
                    {
                        CategoryFilterOptions.Add(category.Name);
                    }
                }
            });
        }

        // ----------------------------
        // Calculation Helpers
        // ----------------------------

        /// <summary>
        /// Calculates totals for expenses and incomes.
        /// </summary>
        private void CalculateAmountSpentAndIncome(List<Transaction> transactions)
        {
            SpentOnLoaded = transactions
                .Where(t => t.Category.Type == "Expense")
                .Sum(t => t.Amount);

            IncomeOnLoaded = transactions
                .Where(t => t.Category.Type == "Income")
                .Sum(t => t.Amount);
        }

        // ----------------------------
        // Property Change Handlers
        // ----------------------------

        partial void OnSelectedCategoryFilterChanged(string value)
        {
            _ = LoadTransactionsAsync();
        }

        partial void OnSelectedTimeFilterChanged(string value)
        {
            _ = LoadTransactionsAsync();
        }

        // ----------------------------
        // Commands
        // ----------------------------

        [RelayCommand]
        private void OpenGraph()
        {
            var window = new GraphView(Transactions);
            window.ShowDialog();
        }

        [RelayCommand]
        private async Task OpenAddCategoryAsync()
        {
            var window = new AddCategoryView(_dbContext, _user);

            if (window.DataContext is AddCategoryViewModel viewmodel)
            {
                viewmodel.OnCategoryAdded = async () => await LoadCategoriesAsync();
            }

            window.ShowDialog();
        }

        [RelayCommand]
        private async Task OpenAddTransaction()
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

        [RelayCommand]
        private async Task ExportDataAsync()
        {
            if (isExportingData)
                return;

            if (Transactions == null || !Transactions.Any())
            {
                MessageBox.Show("There are no transactions to export.", "Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".csv",
                FileName = $"Transactions_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                isExportingData = true;
                try
                {
                    string filePath = saveFileDialog.FileName;
                    StringBuilder csvContent = new StringBuilder();

                    csvContent.AppendLine("Date,Category Name,Category Type,Category Color,Amount");

                    Func<string, string> CsvEscape = (field) =>
                    {
                        if (string.IsNullOrEmpty(field)) return "";
                        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
                        {
                            return $"\"{field.Replace("\"", "\"\"")}\"";
                        }
                        return field;
                    };

                    foreach (var transaction in Transactions)
                    {
                        var date = transaction.Date.ToString("yyyy-MM-dd");
                        var categoryName = transaction.Category?.Name ?? "N/A";
                        var categoryType = transaction.Category?.Type ?? "N/A";
                        var categoryColor = transaction.Category?.Color?.ToString() ?? "N/A";
                        var amount = transaction.Amount.ToString("F2", CultureInfo.InvariantCulture);

                        csvContent.AppendLine($"{date},{CsvEscape(categoryName)},{CsvEscape(categoryType)},{CsvEscape(categoryColor)},{amount}");
                    }

                    await File.WriteAllTextAsync(filePath, csvContent.ToString(), Encoding.UTF8);

                    MessageBox.Show("Data exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    isExportingData = false;
                }
            }
        }
    }
}
