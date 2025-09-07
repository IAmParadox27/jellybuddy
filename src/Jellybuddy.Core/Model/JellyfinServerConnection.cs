using CommunityToolkit.Mvvm.ComponentModel;

namespace Jellybuddy.Core.Model
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
        private string m_deviceId = Guid.NewGuid().ToString().Replace("-", "");

        public override string ToString()
        {
            return $"{Url}";
        }
    }
}