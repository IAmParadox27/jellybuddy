using Jellybuddy.Core.Library;
using Microsoft.Extensions.DependencyInjection;

namespace Jellybuddy.Core.DependencyInjection.Internal
{
    public class Model<T> : IModel<T>
    {
        public T Data { get; }
        
        public Model(IServiceProvider serviceProvider)
        {
            Data = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        }
    }
}