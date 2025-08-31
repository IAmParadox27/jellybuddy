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
            Application.Current.MainPage = ActivatorUtilities.CreateInstance<T>(m_serviceProvider);
        }

        public void NavigateTo<TView, TViewModel>() where TView : Page where TViewModel : class
        {
            TView view = ActivatorUtilities.CreateInstance<TView>(m_serviceProvider);
            view.BindingContext = m_serviceProvider.GetRequiredService<IViewModel<TViewModel>>().Model;

            Application.Current.MainPage = view;
        }
    }
}