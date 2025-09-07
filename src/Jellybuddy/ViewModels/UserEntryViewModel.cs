using System.Net;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Dto;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class UserEntryViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserDto? m_user;
        
        [ObservableProperty]
        private ItemCounts m_itemCounts = new ItemCounts();
        
        [ObservableProperty]
        private IEnumerable<SessionInfoDto> m_userSessions = Array.Empty<SessionInfoDto>();

        [ObservableProperty]
        private ICommand m_editUserCommand;

        [ObservableProperty]
        private ICommand m_deleteUserCommand;

        private readonly JellyfinServerConnection m_server;

        public UserEntryViewModel(JellyfinServerConnection server)
        {
            m_server = server;

            EditUserCommand = new AsyncRelayCommand(OnEditUser);
            DeleteUserCommand = new AsyncRelayCommand(OnDeleteUser);
        }

        private async Task OnDeleteUser()
        {
            if (User == null)
            {
                return;
            }
            
            // TODO: Add some kind of confirmation dialog.

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(m_server.Url!);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{m_server.DeviceId}\", Version=\"1.0.0\", Token=\"{m_server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"/Users/{User.Id}");

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    string? json = await response.Content.ReadAsStringAsync();

                    if (json != null)
                    {
                        ProblemDetails? problemDetails = JObject.Parse(json).ToObject<ProblemDetails>();
                        
                        // TODO: Show error message
                    }
                }
                else
                {
                    // Delete was successful.
                    User = null;
                    UserSessions = Array.Empty<SessionInfoDto>();
                    ItemCounts = new ItemCounts();
                }
            }
            catch (Exception)
            {
                _ = 12;
            }
        }

        private async Task OnEditUser()
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
        }
        
        partial void OnUserChanged(UserDto? value)
        {
            if (value != null)
            {
                Thread t = new Thread(async void () =>
                {
                    try
                    {
                        await LoadItemCountsAsync().ConfigureAwait(false);
                        await LoadUserActiveSessionsAsync().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                    }
                });
                t.Start();
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
    }
}