using ExpenseTracker.Data;
using ExpenseTracker.Services;

namespace ExpenseTracker.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private readonly AppDbContext _dbContext;

        public HomeViewModel(NavigationService ns, AppDbContext db)
        {
            _navigationService = ns;
            _dbContext = db;
        }
    }
}
