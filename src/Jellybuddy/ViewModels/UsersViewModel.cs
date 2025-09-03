using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellybuddy.Dto;
using Jellybuddy.Models;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;
using Rotorsoft.Maui;

namespace Jellybuddy.ViewModels
{
    public partial class UsersViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private ObservableCollection<UserDto> m_users = new ObservableCollection<UserDto>();

        [ObservableProperty]
        private string? m_searchText;
        
        [ObservableProperty]
        private CollectionViewSource m_usersViewSource;
        
        private readonly IModel<DataCache> m_dataCache;
        private readonly IUIContext m_uiContext;

        public UsersViewModel(IModel<DataCache> dataCache, IUIContext uiContext)
        {
            m_dataCache = dataCache;
            m_uiContext = uiContext;

            UsersViewSource = new CollectionViewSource
            {
                Source = Users,
                Filter = x =>
                {
                    if (x is UserDto user && !string.IsNullOrEmpty(SearchText))
                    {
                        if (!user.Name.ToLower().Contains(SearchText.ToLower()))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            };
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
                    UserDto[] usersResult = JArray.Parse(json).ToObject<UserDto[]>() ?? Array.Empty<UserDto>();
                    
                    foreach (UserDto user in usersResult)
                    {
                        Users.Add(user);
                    }
                }
            }
        }

        partial void OnSearchTextChanged(string? value)
        {
            UsersViewSource.View.Refresh();
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