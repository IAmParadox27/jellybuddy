using Jellybuddy.Core.Library;

namespace Jellybuddy.Services
{
    public class UIContext : IUIContext
    {
        public IDispatcher Dispatcher => Application.Current!.Dispatcher;

        public void Run(Action action)
        {
            action();
            //Dispatcher.Dispatch(action);
        }
    }
}