using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellybuddy.Core.Model;
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
        private RangedObservableCollection<UserEntryViewModel> m_users = new RangedObservableCollection<UserEntryViewModel>();

        [ObservableProperty]
        private string? m_searchText;

        [ObservableProperty]
        private bool m_isRefreshing = false;
        
        [ObservableProperty]
        private CollectionViewSource m_usersViewSource;

        [ObservableProperty]
        private ICommand m_changeSortCategory;

        [ObservableProperty]
        private ICommand m_changeSortDirection;

        [ObservableProperty]
        private ICommand m_refreshCommand;
        
        [ObservableProperty]
        private IServerConnectionManager m_serverConnectionManager;
        
        private readonly IUIContext m_uiContext;
        private readonly IServiceProvider m_serviceProvider;

        public UsersViewModel(IUIContext uiContext, IServiceProvider serviceProvider, IServerConnectionManager serverConnectionManager)
        {
            m_uiContext = uiContext;
            m_serviceProvider = serviceProvider;
            
            ServerConnectionManager = serverConnectionManager;

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
                    new SortDescription($"{nameof(UserEntryViewModel.User)}.{nameof(UserDto.Name)}", ListSortDirection.Ascending)
                }
            };

            ChangeSortCategory = new RelayCommand<UserSortCategory>(OnChangeSortCategory);
            ChangeSortDirection = new RelayCommand<ListSortDirection>(OnChangeSortDirection);
            RefreshCommand = new AsyncRelayCommand(OnRefresh);
        }

        private async Task OnRefresh()
        {
            if (ServerConnectionManager.ActiveConnection != null)
            {
                await LoadUsersAsync(ServerConnectionManager.ActiveConnection, false).ConfigureAwait(false);
            }
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
                UserSortCategory.Name => $"{nameof(UserEntryViewModel.User)}.{nameof(UserDto.Name)}",
                UserSortCategory.LastActive => $"{nameof(UserEntryViewModel.User)}.{nameof(UserDto.LastActivityDate)}",
                UserSortCategory.Role => $"{nameof(UserEntryViewModel.User)}.{nameof(UserDto.Policy)}.{nameof(UserPolicy.IsAdministrator)}"
            }, sortDescription.Direction));
        }

        private async Task LoadUsersAsync(JellyfinServerConnection server, bool isBackgroundRefresh = true)
        {
            if (server.Url == null)
            {
                return;
            }

            if (!isBackgroundRefresh)
            {
                IsRefreshing = true;
            }
            
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
                    UserEntryViewModel[] userEntryViewModels = usersResult.Select(x =>
                    {
                        UserEntryViewModel userEntryViewModel = ActivatorUtilities.CreateInstance<UserEntryViewModel>(m_serviceProvider);
                        userEntryViewModel.User = x;
                            
                        userEntryViewModel.UserDeleted += UserEntryViewModel_OnUserDeleted;
                            
                        return userEntryViewModel;
                    }).ToArray();
                    
                    Users.Clear();
                    Users.AddRange(userEntryViewModels);
                    
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UsersViewSource.View.Refresh();
                    });
                }
            }

            if (!isBackgroundRefresh)
            {
                IsRefreshing = false;
            }
        }

        private void UserEntryViewModel_OnUserDeleted(UserEntryViewModel userEntryViewModel)
        {
            Users.Remove(userEntryViewModel);
        }

        partial void OnSearchTextChanged(string? value)
        {
            UsersViewSource.View.Refresh();
        }

        public void OnNavigatedTo()
        {
            Thread thread = new Thread(async () =>
            {
                if (ServerConnectionManager.ActiveConnection != null)
                {
                    await LoadUsersAsync(ServerConnectionManager.ActiveConnection, false).ConfigureAwait(false);
                }
            });
            thread.Start();
        }

        public void OnNavigatedFrom()
        {
        }
    }
}