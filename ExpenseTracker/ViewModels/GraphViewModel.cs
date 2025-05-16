using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModels
{
    public partial class GraphViewModel : ObservableObject
    {
        // ----------------------------
        // Properties
        // ----------------------------

        // Collection of transactions to be visualized
        public ObservableCollection<Transaction> Transactions { get; }

        // Chart data series collection for the pie chart
        public SeriesCollection SeriesCollection { get; set; }

        // ----------------------------
        // Constructor
        // ----------------------------

        // Initializes the ViewModel with transaction data and prepares the pie chart series
        public GraphViewModel(ObservableCollection<Transaction> transactions)
        {
            Transactions = transactions;

            // Group expense transactions by category and sum their amounts
            var groupedData = Transactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(t => t.Amount)
                })
                .Where(g => g.Total > 0) // Only include categories with positive totals
                .ToList();

            SeriesCollection = new SeriesCollection();

            // Create pie chart series for each category group
            foreach (var data in groupedData)
            {
                // Attempt to get the category color, fallback to gray if unavailable
                var colorHex = Transactions
                    .FirstOrDefault(t => t.Category.Name == data.Category)?.Category.Color ?? "#CCCCCC";

                SeriesCollection.Add(new PieSeries
                {
                    Title = data.Category,
                    Values = new ChartValues<double> { (double)data.Total },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y:C}", // Format labels as currency
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorHex) ?? Brushes.Gray)
                });
            }
        }
    }
}
