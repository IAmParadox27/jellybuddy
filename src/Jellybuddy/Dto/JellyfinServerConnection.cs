using CommunityToolkit.Mvvm.ComponentModel;

namespace Jellybuddy.Dto
{
    public partial class JellyfinServerConnection : ObservableObject
    {
        [ObservableProperty]
        private string? m_url;
        
        [ObservableProperty]
        private string? m_username;
        
        [ObservableProperty]
        private string? m_password; // Might remove this not sure yet
        
        [ObservableProperty]
        private string? m_accessToken;

        [ObservableProperty]
        private string m_deviceId;
    }
}