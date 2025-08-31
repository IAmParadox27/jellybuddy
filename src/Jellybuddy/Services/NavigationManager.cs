using Jellybuddy.Core.DependencyInjection;

namespace Jellybuddy.Services
{
    public class NavigationManager : INavigationManager
    {
        private readonly IServiceProvider m_serviceProvider;

        public NavigationManager(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider;
        }
        
        public void NavigateTo<T>() where T : Page
        {
            ChangeToView(ActivatorUtilities.CreateInstance<T>(m_serviceProvider));
        }

        public void NavigateTo<TView, TViewModel>() where TView : Page where TViewModel : class
        {
            TView view = ActivatorUtilities.CreateInstance<TView>(m_serviceProvider);
            view.BindingContext = m_serviceProvider.GetRequiredService<IViewModel<TViewModel>>().Model;

            ChangeToView(view);
        }

        private void ChangeToView<T>(T view) where T : Page
        {
            if (Application.Current!.MainPage != null && Application.Current!.MainPage.BindingContext is IPageViewModel fromPageViewModel)
            {
                fromPageViewModel.OnNavigatedFrom();
            }
            
            Application.Current!.MainPage = view;
            
            if (view.BindingContext is IPageViewModel toPageViewModel)
            {
                toPageViewModel.OnNavigatedTo();
            }
        }
    }
}