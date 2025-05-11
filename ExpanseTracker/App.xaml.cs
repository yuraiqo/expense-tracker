using ExpenseTracker.Data;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using ExpenseTracker.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace ExpenseTracker;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // set up DbContextOptions 
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=expense_tracker;Username=et_root;Password=mypass123");

        var dbContext = new AppDbContext(optionsBuilder.Options);

        // set up navigation and initiate default view with dbContext
        NavigationService navigationService = new NavigationService();

        navigationService.CurrentViewModel = new LoginViewModel(navigationService, dbContext);

        MainView mv = new MainView()
        {
            DataContext = new MainViewModel(navigationService)
        };
        mv.Show();

        base.OnStartup(e);
    }
}

