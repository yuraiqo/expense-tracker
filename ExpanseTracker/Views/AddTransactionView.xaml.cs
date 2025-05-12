using System.Windows;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Views
{
    public partial class AddTransactionView : Window
    {
        public AddTransactionView(AppDbContext db, User user)
        {
            InitializeComponent();
            DataContext = new AddTransactionViewModel(db, user, this);
        }
    }
}
