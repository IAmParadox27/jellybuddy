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

            VirtualFolderInfo[] libraries = await LoadLibrariesAsync();

            await Parallel.ForEachAsync(libraries, async (library, _) =>
            {
                switch (library.CollectionType)
                {
                    case VirtualFolderInfoCollectionType.Movies:
                    {
                        int count = await LoadCountForLibraryAsync(library, BaseItemKind.Movie);
                        ItemCounts.MovieCount += count;
                        break;
                    }
                    case VirtualFolderInfoCollectionType.Tvshows:
                    {
                        int seriesCount = await LoadCountForLibraryAsync(library, BaseItemKind.Series);
                        int episodeCount = await LoadCountForLibraryAsync(library, BaseItemKind.Episode);
                        
                        ItemCounts.SeriesCount += seriesCount;
                        ItemCounts.EpisodeCount += episodeCount;
                        break;
                    }
                    case VirtualFolderInfoCollectionType.Boxsets:
                    {
                        int count = await LoadCountForLibraryAsync(library, BaseItemKind.BoxSet);
                        ItemCounts.BoxSetCount += count;
                        break;
                    }
                    default:
                        break;
                }
            });
        }

        private async Task<VirtualFolderInfo[]> LoadLibrariesAsync()
        {
            HttpResponseMessage response = await ServerConnectionManager.ActiveConnectionClient!.GetAsync("/Library/VirtualFolders");

            string responseJson = await response.Content.ReadAsStringAsync();

            VirtualFolderInfo[]? libraries = JArray.Parse(responseJson).ToObject<VirtualFolderInfo[]>();

            return libraries ?? Array.Empty<VirtualFolderInfo>();
        }

        private async Task<int> LoadCountForLibraryAsync(VirtualFolderInfo library, BaseItemKind kind)
        {
            HttpResponseMessage response = await ServerConnectionManager.ActiveConnectionClient!.GetAsync($"/Users/{User.Id}/Items?IncludeItemTypes={kind}&Recursive=true&StartIndex=0&Limit=1&ParentId={library.ItemId}");

            string responseJson = await response.Content.ReadAsStringAsync();
            
            return JObject.Parse(responseJson).Value<int>("TotalRecordCount");
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