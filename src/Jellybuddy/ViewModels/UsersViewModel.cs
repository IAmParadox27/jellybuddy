using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Dto;
using Jellybuddy.Models;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class UsersViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private ObservableCollection<UserDto> m_users = new ObservableCollection<UserDto>();

        private readonly IModel<DataCache> m_dataCache;

        public UsersViewModel(IModel<DataCache> dataCache)
        {
            m_dataCache = dataCache;
        }
        
        private async Task LoadUsersAsync(JellyfinServerConnection server)
        {
            if (server.Url == null)
            {
                return;
            }
            
            Users.Clear();
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(server.Url);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"MyDeviceName\", DeviceId=\"a7037817ace34ddc9d82385c63ac5f33\", Version=\"1.0.0\", Token=\"{server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            HttpResponseMessage response = await client.GetAsync("/Users");

            if (response.IsSuccessStatusCode)
            {
                string? json = await response.Content.ReadAsStringAsync();

                if (json != null)
                {
                    foreach (UserDto user in (JArray.Parse(json).ToObject<UserDto[]>() ?? Array.Empty<UserDto>()))
                    {
                        Users.Add(user);
                    }
                }
            }
        }

        public void OnNavigatedTo()
        {
            _ = LoadUsersAsync(m_dataCache.Data.Servers.First());
        }

        public void OnNavigatedFrom()
        {
        }
    }
}