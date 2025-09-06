using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Dto;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.Models
{
    public partial class UserDetails : ObservableObject, IEquatable<UserDetails>
    {
        [ObservableProperty]
        private UserDto m_user;

        [ObservableProperty]
        private ItemCounts m_itemCounts;

        public int TotalVideoCount => ItemCounts.EpisodeCount + ItemCounts.MovieCount;

        public UserDetails(JellyfinServerConnection server, UserDto user)
        {
            User = user;
            ItemCounts = new ItemCounts();
            
            PropertyChanged += OnPropertyChanged;
            
            _ = LoadItemCountsAsync(server);
        }

        private async Task LoadItemCountsAsync(JellyfinServerConnection server)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(server.Url!);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{server.DeviceId}\", Version=\"1.0.0\", Token=\"{server.AccessToken}\"");
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
            catch (Exception e)
            {
                _ = 12;
            }
        }
        
        partial void OnItemCountsChanged(ItemCounts? oldValue, ItemCounts newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= ItemCounts_PropertyChanged;
            }
            
            newValue.PropertyChanged += ItemCounts_PropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalVideoCount));
        }

        private void ItemCounts_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalVideoCount));
        }

        public bool Equals(UserDetails? other)
        {
            return User.Id == other?.User.Id;
        }
    }
}