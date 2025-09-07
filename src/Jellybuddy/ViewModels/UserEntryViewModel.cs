using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Dto;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class UserEntryViewModel : ObservableObject, IComparable
    {
        [ObservableProperty]
        private UserDto? m_user;
        
        [ObservableProperty]
        private ItemCounts m_itemCounts = new ItemCounts();
        
        [ObservableProperty]
        private IEnumerable<SessionInfoDto> m_userSessions = Array.Empty<SessionInfoDto>();

        private readonly JellyfinServerConnection m_server;

        public UserEntryViewModel(JellyfinServerConnection server)
        {
            m_server = server;
        }

        partial void OnUserChanged(UserDto? value)
        {
            if (value != null)
            {
                _ = LoadItemCountsAsync().ConfigureAwait(false);
                _ = LoadUserActiveSessionsAsync().ConfigureAwait(false);
            }
        }

        private async Task LoadItemCountsAsync()
        {
            if (User == null)
            {
                return;
            }
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(m_server.Url!);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{m_server.DeviceId}\", Version=\"1.0.0\", Token=\"{m_server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"/Items/Counts?userId={User.Id}");

                if (response.IsSuccessStatusCode)
                {
                    string? json = await response.Content.ReadAsStringAsync();

                    if (json != null)
                    {
                        ItemCounts = JObject.Parse(json).ToObject<ItemCounts>() ?? new ItemCounts();
                    }
                }
            }
            catch (Exception)
            {
                _ = 12;
            }
        }

        private async Task LoadUserActiveSessionsAsync()
        {
            if (User == null)
            {
                return;
            }
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(m_server.Url!);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{m_server.DeviceId}\", Version=\"1.0.0\", Token=\"{m_server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"/Sessions?controllableByUserId={User.Id}");

                if (response.IsSuccessStatusCode)
                {
                    string? json = await response.Content.ReadAsStringAsync();

                    if (json != null)
                    {
                        UserSessions = JArray.Parse(json).ToObject<SessionInfoDto[]>() ?? Array.Empty<SessionInfoDto>();
                    }
                }
            }
            catch (Exception)
            {
                _ = 12;
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj is UserEntryViewModel other)
            {
                if (User != null && other.User?.Id == User?.Id)
                {
                    return 0;
                }

                if (User != null && other.User != null)
                {
                    return string.Compare(User.Name, other.User.Name, StringComparison.Ordinal);
                }

                // Treat them in the same place if they're not ready.
                return 0;
            }

            // Always return -1 to ensure that the user is always at the top of the list
            return -1;
        }
    }
}