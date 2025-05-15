using System.Windows;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Views
{
    public partial class AddCategoryView : Window
    {
        public AddCategoryView(AppDbContext db, User user)
        {
            InitializeComponent();
            DataContext = new AddCategoryViewModel(db, user, this);
        }
    }
}
