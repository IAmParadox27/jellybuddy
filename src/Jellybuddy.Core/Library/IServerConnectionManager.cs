using System.ComponentModel;
using Jellybuddy.Core.Model;

namespace Jellybuddy.Core.Library
{
    public interface IServerConnectionManager : INotifyPropertyChanged
    {
        JellyfinServerConnection? ActiveConnection { get; }
        
        IList<JellyfinServerConnection> Servers { get; }
        
        HttpClient? ActiveConnectionClient { get; }
        
        Task ChangeActiveConnection(JellyfinServerConnection connection);
        
        Task AddServerAsync(JellyfinServerConnection server);
        
        Task RemoveServer(JellyfinServerConnection server);
    }
}