using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Jellybuddy.Core.DependencyInjection.Internal
{
    internal partial class ViewModel<T> : ObservableObject, IViewModel<T> where T : class
    {
        [ObservableProperty]
        private T m_model;

        public ViewModel(IServiceProvider serviceProvider)
        {
            Model = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }
}