using ExpenseTracker.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ExpenseTracker.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
    }
}
