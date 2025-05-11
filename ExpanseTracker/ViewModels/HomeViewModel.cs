using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services;

namespace ExpenseTracker.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        private readonly AppDbContext _dbContext;

        private readonly User _user;

        public HomeViewModel(NavigationService ns, AppDbContext db, User user)
        {
            _navigationService = ns;
            _dbContext = db;
            _user = user;
        }

        public string Username => _user.Username;
    }
}
