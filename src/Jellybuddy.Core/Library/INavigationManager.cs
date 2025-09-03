namespace Jellybuddy.Core.Library
{
    public interface INavigationManager<TPageType>
    {
        public Task NavigateToAsync<T>() 
            where T : TPageType;
    }
}