using ExpenseTracker.Services;

namespace ExpenseTracker.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        public HomeViewModel(NavigationService ns)
        {
            _navigationService = ns;
        }
    }
}
