using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services
{
    public class NavigationService
    {
        public event Action? CurrentViewModelChanged;

        private ViewModelBase? _currentViewModel;

        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
