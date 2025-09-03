namespace Jellybuddy.Core.Library
{
    public interface IViewModel<T> where T : class
    {
        T Model { get; }
    }
}