using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Services;

namespace Jellybuddy.Pages
{
    static class ViewModelPageExtensions
    {
        internal static object? GetViewModel(this object? bindingContext)
        {
            if (bindingContext != null)
            {
                Type? genericType = bindingContext.GetType().GenericTypeArguments.FirstOrDefault();
                
                if (genericType != null && bindingContext.GetType().IsAssignableTo(typeof(IViewModel<>).MakeGenericType(genericType)))
                {
                    return bindingContext.GetType().GetProperty("Model")?.GetValue(bindingContext);
                }
            }

            return null;
        }
    }
    
    public class ServiceContentPage : ContentPage
    {
        protected override void OnHandlerChanged()
        {
            Type? viewModelInterface = GetType().GetInterfaces().FirstOrDefault(x => x.Name == typeof(IViewModelPage<>).Name);
            
            if (viewModelInterface == null)
            {
                throw new InvalidDataException("IViewModelPage<ViewModel> is required for ServiceContentPage types");
            }
            
            Type viewModelType = typeof(IViewModel<>).MakeGenericType(viewModelInterface.GenericTypeArguments[0]);
            
            BindingContext = Handler?.MauiContext?.Services.GetRequiredService(viewModelType).GetViewModel();

            if (BindingContext != null)
            {
                if (BindingContext is IPageViewModel pageViewModel)
                {
                    pageViewModel.OnNavigatedTo();
                }
            }
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is IPageViewModel pageViewModel)
            {
                pageViewModel.OnNavigatedTo();
            }
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            if (BindingContext is IPageViewModel pageViewModel)
            {
                pageViewModel.OnNavigatedFrom();
            }
            
            base.OnNavigatedFrom(args);
        }
    }
}