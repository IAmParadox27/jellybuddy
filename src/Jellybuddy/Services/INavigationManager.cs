namespace Jellybuddy.Services
{
    public interface INavigationManager
    {
        public Task NavigateToAsync<T>() 
            where T : Page;
    }
}