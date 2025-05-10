using ExpenseTracker.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ExpenseTracker.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }
        
        private void SwitchToLogin(object sender, RequestNavigateEventArgs e)
        {
            
        }
    }
}
