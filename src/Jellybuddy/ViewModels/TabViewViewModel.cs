using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.DependencyInjection;

namespace Jellybuddy.ViewModels
{
    public partial class TabViewViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private IViewModel<ActiveSessionsViewModel> m_activeSessionsViewModel;
        
        [ObservableProperty]
        private IViewModel<UsersViewModel> m_usersViewModel;
        
        [ObservableProperty]
        private Page? m_selectedTab;

        public TabViewViewModel(IViewModel<ActiveSessionsViewModel> activeSessionsViewModel, IViewModel<UsersViewModel> usersViewModel)
        {
            ActiveSessionsViewModel = activeSessionsViewModel;
            UsersViewModel = usersViewModel;
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

        partial void OnSelectedTabChanged(Page? oldValue, Page? newValue)
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