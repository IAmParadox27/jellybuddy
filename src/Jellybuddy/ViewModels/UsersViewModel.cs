using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellybuddy.Dto;
using Jellybuddy.Models;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;
using Rotorsoft.Maui;

namespace Jellybuddy.ViewModels
{
    public enum UserSortCategory
    {
        Name,
        LastActive,
        Role
    }
    
    public partial class UsersViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private RangedObservableCollection<UserDto> m_users = new RangedObservableCollection<UserDto>();

        [ObservableProperty]
        private RangedObservableCollection<(Guid UserId, ItemCounts Counts)> m_itemCounts = new RangedObservableCollection<(Guid UserId, ItemCounts Counts)>();
        
        [ObservableProperty]
        private string? m_searchText;
        
        [ObservableProperty]
        private CollectionViewSource m_usersViewSource;

        [ObservableProperty]
        private ICommand m_changeSortCategory;

        [ObservableProperty]
        private ICommand m_changeSortDirection;
        
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
                },
                SortDescriptions =
                {
                    new SortDescription($"{nameof(UserDto.Name)}", ListSortDirection.Ascending)
                }
            };

            ChangeSortCategory = new RelayCommand<UserSortCategory>(OnChangeSortCategory);
            ChangeSortDirection = new RelayCommand<ListSortDirection>(OnChangeSortDirection);
        }

        private void OnChangeSortDirection(ListSortDirection obj)
        {
            SortDescription sortDescription = UsersViewSource.SortDescriptions.First();
            
            UsersViewSource.SortDescriptions.Clear();
            UsersViewSource.SortDescriptions.Add(new SortDescription(sortDescription.PropertyName, obj));
        }

        private void OnChangeSortCategory(UserSortCategory obj)
        {
            SortDescription sortDescription = UsersViewSource.SortDescriptions.First();
            
            UsersViewSource.SortDescriptions.Clear();
            UsersViewSource.SortDescriptions.Add(new SortDescription(obj switch
            {
                UserSortCategory.Name => $"{nameof(UserDto.Name)}",
                UserSortCategory.LastActive => $"{nameof(UserDto.LastActivityDate)}",
                UserSortCategory.Role => $"{nameof(UserDto.Policy)}.{nameof(UserPolicy.IsAdministrator)}"
            }, sortDescription.Direction));
        }

        private async Task LoadUsersAsync(JellyfinServerConnection server)
        {
            if (server.Url == null)
            {
                return;
            }
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Users.Clear();
            });
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(server.Url);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{server.DeviceId}\", Version=\"1.0.0\", Token=\"{server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            HttpResponseMessage response = await client.GetAsync("/Users");

            List<Task<(UserDto user, ItemCounts? counts)>> itemCountsTasks = new List<Task<(UserDto user, ItemCounts? counts)>>();
            if (response.IsSuccessStatusCode)
            {
                string? json = await response.Content.ReadAsStringAsync();

                if (json != null)
                {
                    UserDto[] usersResult = JArray.Parse(json).ToObject<UserDto[]>() ?? Array.Empty<UserDto>();
                    
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Users.AddRange(usersResult);
                    });
                    
                    foreach (UserDto user in usersResult)
                    {
                        itemCountsTasks.Add(LoadItemCountsAsync(server, user).ContinueWith(x => (user, x.Result)));
                    }
                }
            }

            _ = Task.WhenAll(itemCountsTasks).ContinueWith(x =>
            {
                ItemCounts.AddRange(x.Result.Select(y => (y.user.Id, y.counts ?? new ItemCounts())));
            });
        }

        partial void OnSearchTextChanged(string? value)
        {
            UsersViewSource.View.Refresh();
        }

        public void OnNavigatedTo()
        {
            Thread thread = new Thread(async () =>
            {
                await LoadUsersAsync(m_dataCache.Data.Servers.First()).ConfigureAwait(false);
            });
            thread.Start();
        }

        public void OnNavigatedFrom()
        {
        }
        
        private async Task<ItemCounts?> LoadItemCountsAsync(JellyfinServerConnection server, UserDto user)
        {
            ItemCounts? itemCounts = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(server.Url!);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{server.DeviceId}\", Version=\"1.0.0\", Token=\"{server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"/Items/Counts?userId={user.Id}");

                if (response.IsSuccessStatusCode)
                {
                    string? json = await response.Content.ReadAsStringAsync();

                    if (json != null)
                    {
                        itemCounts = JObject.Parse(json).ToObject<ItemCounts>() ?? new ItemCounts();
                    }
                }
            }
            catch (Exception e)
            {
                _ = 12;
            }

            return itemCounts;
        }
    }
}