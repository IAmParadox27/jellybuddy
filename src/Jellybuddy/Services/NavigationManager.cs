using System.Reflection;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellybuddy.Pages;

namespace Jellybuddy.Services
{
    public class NavigationManager : INavigationManager<Page>
    {
        private readonly IServiceProvider m_serviceProvider;
        
        public NavigationManager(IServiceProvider serviceProvider)
        {
            m_serviceProvider = serviceProvider;
        }
        
        public Task NavigateToAsync<T>() where T : Page
        {
            return ChangeToViewAsync<T>();
        }

        private async Task ChangeToViewAsync<T>() where T : Page
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                Shell.Current.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync($"///{typeof(T).Name}");
                });
            }
            else
            {
                await Shell.Current.GoToAsync($"///{typeof(T).Name}");
            }
        }
    }
}