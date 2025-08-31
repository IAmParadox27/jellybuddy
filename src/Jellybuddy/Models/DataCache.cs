using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Dto;

namespace Jellybuddy.Models
{
    public partial class DataCache : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<JellyfinServerConnection> m_servers = new ObservableCollection<JellyfinServerConnection>();
    }
}