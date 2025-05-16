using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExpenseTracker.Views
{
    public partial class GraphView : Window
    {
        public GraphView(ObservableCollection<Transaction> transactions)
        {
            InitializeComponent();
            DataContext = new GraphViewModel(transactions);
        }
    }
}
