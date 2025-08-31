using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.DependencyInjection;

namespace Jellybuddy.ViewModels
{
    public partial class TabViewViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private IViewModel<ActiveSessionsViewModel> m_activeSessionsViewModel;
        
        [ObservableProperty]
        private Page? m_selectedTab;

        public TabViewViewModel(IViewModel<ActiveSessionsViewModel> activeSessionsViewModel)
        {
            ActiveSessionsViewModel = activeSessionsViewModel;
        }
        
        public void OnNavigatedTo()
        {
            if (SelectedTab?.BindingContext is IPageViewModel toPageViewModel)
            {
                toPageViewModel.OnNavigatedTo();
            }
        }

        public void OnNavigatedFrom()
        {
            if (SelectedTab?.BindingContext is IPageViewModel toPageViewModel)
            {
                toPageViewModel.OnNavigatedFrom();
            }
        }

        partial void OnSelectedTabChanged(Page? oldValue, Page? newValue)
        {
            if (oldValue?.BindingContext is IPageViewModel oldPageViewModel)
            {
                oldPageViewModel.OnNavigatedFrom();
            }

            if (newValue?.BindingContext is IPageViewModel newPageViewModel)
            {
                newPageViewModel.OnNavigatedTo();
            }
        }
    }
}