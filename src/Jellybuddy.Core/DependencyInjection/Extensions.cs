using Jellybuddy.Core.DependencyInjection.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Jellybuddy.Core.DependencyInjection
{
    public static class Extensions
    {
        public static MauiAppBuilder UseViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton(typeof(IViewModel<>), typeof(ViewModel<>));
            builder.Services.AddSingleton(typeof(IModel<>), typeof(Model<>));
            
            return builder;
        }
    }
}