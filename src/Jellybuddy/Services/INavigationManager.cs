namespace Jellybuddy.Services
{
    public interface INavigationManager
    {
        public void NavigateTo<T>() 
            where T : Page;
        
        public void NavigateTo<TView, TViewModel>() 
            where TView : Page 
            where TViewModel : class;
    }
}