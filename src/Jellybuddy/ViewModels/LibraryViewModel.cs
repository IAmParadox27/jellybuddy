using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.Library;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class LibraryViewModel : ObservableObject, IPageViewModel
    {
        private readonly IServerConnectionManager m_serverConnectionManager;
        private readonly IUIContext m_uiContext;

        [ObservableProperty]
        private RangedObservableCollection<VirtualFolderInfo> m_libraries = new RangedObservableCollection<VirtualFolderInfo>();

        [ObservableProperty]
        private bool m_isRefreshing = false;
        
        [ObservableProperty]
        private ICommand m_refreshCommand;

        public LibraryViewModel(IServerConnectionManager serverConnectionManager, IUIContext uiContext)
        {
            m_serverConnectionManager = serverConnectionManager;
            m_uiContext = uiContext;

            RefreshCommand = new AsyncRelayCommand(OnRefresh);
        }

        private Task OnRefresh()
        {
            return Task.CompletedTask;
        }

        public void OnNavigatedTo()
        {
            Thread thread = new Thread(async () =>
            {
                await LoadDataAsync().ConfigureAwait(false);
            });
            thread.Start();
        }

        public void OnNavigatedFrom()
        {
        }

        private async Task LoadDataAsync()
        {
            if (m_serverConnectionManager.ActiveConnectionClient == null)
            {
                return;
            }
            
            HttpResponseMessage response = await m_serverConnectionManager.ActiveConnectionClient.GetAsync("/Library/VirtualFolders");

            string responseJson = await response.Content.ReadAsStringAsync();

            VirtualFolderInfo[]? libraries = JArray.Parse(responseJson).ToObject<VirtualFolderInfo[]>();

            m_uiContext.Run(() =>
            {
                Libraries.Clear();
                if (libraries != null)
                {
                    Libraries.AddRange(libraries);
                }
            });
        }
    }
}