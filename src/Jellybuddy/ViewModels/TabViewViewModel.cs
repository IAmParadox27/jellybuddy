using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;

namespace Jellybuddy.ViewModels
{
    public partial class TabViewViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private IViewModel<ActiveSessionsViewModel> m_activeSessionsViewModel;
        
        [ObservableProperty]
        private IViewModel<UsersViewModel> m_usersViewModel;
        
        [ObservableProperty]
        private IViewModel<SettingsViewModel> m_settingsViewModel;
        
        [ObservableProperty]
        private View? m_selectedTab;

        public TabViewViewModel(IViewModel<ActiveSessionsViewModel> activeSessionsViewModel, IViewModel<UsersViewModel> usersViewModel, IViewModel<SettingsViewModel> settingsViewModel)
        {
            ActiveSessionsViewModel = activeSessionsViewModel;
            UsersViewModel = usersViewModel;
            SettingsViewModel = settingsViewModel;
        }
        
        public void OnNavigatedTo()
        {
            if (SelectedTab?.BindingContext is IPageViewModel toPageViewModel && toPageViewModel != this)
            {
                toPageViewModel.OnNavigatedTo();
            }
        }

        public void OnNavigatedFrom()
        {
            if (SelectedTab?.BindingContext is IPageViewModel toPageViewModel && toPageViewModel != this)
            {
                toPageViewModel.OnNavigatedFrom();
            }
        }

        partial void OnSelectedTabChanged(View? oldValue, View? newValue)
        {
            if (oldValue?.BindingContext is IPageViewModel oldPageViewModel && oldPageViewModel != this)
            {
                oldPageViewModel.OnNavigatedFrom();
            }

            if (newValue?.BindingContext is IPageViewModel newPageViewModel && newPageViewModel != this)
            {
                newPageViewModel.OnNavigatedTo();
            }
        }
    }
}