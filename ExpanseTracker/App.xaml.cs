using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using ExpenseTracker.Views;
using System.Windows;

namespace ExpenseTracker;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        NavigationService navigationService = new NavigationService();

        navigationService.CurrentViewModel = new LoginViewModel(navigationService);

        MainView mv = new MainView()
        {
            DataContext = new MainViewModel(navigationService)
        };
        mv.Show();

        base.OnStartup(e);
    }
}

