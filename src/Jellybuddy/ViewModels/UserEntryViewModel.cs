using System.Net;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.Library;
using Jellybuddy.Core.Model;
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

        [ObservableProperty]
        private IServerConnectionManager m_serverConnectionManager;

        public event Action<UserEntryViewModel>? UserDeleted;
        
        public UserEntryViewModel(IServerConnectionManager serverConnectionManager)
        {
            ServerConnectionManager = serverConnectionManager;

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
            try
            {
                HttpResponseMessage response = await ServerConnectionManager.ActiveConnectionClient!.DeleteAsync($"/Users/{User.Id}");

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
                    
                    UserDeleted?.Invoke(this);
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
            
            try
            {
                HttpResponseMessage response = await ServerConnectionManager.ActiveConnectionClient!.GetAsync($"/Items/Counts?userId={User.Id}");

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
            
            try
            {
                HttpResponseMessage response = await ServerConnectionManager.ActiveConnectionClient!.GetAsync($"/Sessions?controllableByUserId={User.Id}");

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