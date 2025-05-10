using ExpenseTracker.ViewModels;
using System.Windows;

namespace ExpenseTracker.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
