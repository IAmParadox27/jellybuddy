namespace Jellybuddy.Core.DependencyInjection
{
    public interface IViewModel<T> where T : class
    {
        T Model { get; }
    }
}