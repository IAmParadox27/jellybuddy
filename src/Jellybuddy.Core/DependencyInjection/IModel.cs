namespace Jellybuddy.Core.DependencyInjection
{
    public interface IModel<T>
    {
        T Data { get; }
    }
}