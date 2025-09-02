using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Dto;
using Jellybuddy.Models;
using Jellybuddy.Services;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class ActiveSessionsViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private ObservableCollection<ActiveSessionGroup> m_sessions = new ObservableCollection<ActiveSessionGroup>();

        [ObservableProperty]
        private ObservableCollection<SessionInfoDto> m_allSessions = new ObservableCollection<SessionInfoDto>();
        
        [ObservableProperty]
        private ICommand m_refreshCommand;
        
        [ObservableProperty]
        private bool m_isRefreshing = false;
        
        private readonly IModel<DataCache> m_model;
        private readonly INavigationManager m_navigationManager;
        private readonly IUIContext m_uiContext;

        public ActiveSessionsViewModel(IModel<DataCache> model, INavigationManager navigationManager, IUIContext uiContext)
        {
            m_model = model;
            m_navigationManager = navigationManager;
            m_uiContext = uiContext;

            RefreshCommand = new AsyncRelayCommand(LoadSessionsAsync);
        }

        private bool m_isTimerRefreshing = false;

        private async Task RefreshTimer()
        {
            if (m_isTimerRefreshing)
            {
                // Don't allow a second timer to be started.
                return;
            }
            
            m_isTimerRefreshing = true;
            while (m_isTimerRefreshing)
            {
                try
                {
                    await LoadSessionsAsync(false);
                }
                catch
                {
                }
            }
        }

        private Task LoadSessionsAsync()
        {
            return LoadSessionsAsync(true);
        }

        private async Task LoadSessionsAsync(bool isRefreshing)
        {
            if (isRefreshing)
            {
                m_uiContext.Run(() => IsRefreshing = true);
            }
            
            foreach (JellyfinServerConnection serverConnection in m_model.Data.Servers)
            {
                try
                {
                    await GetSessionsForServerAsync(serverConnection);
                }
                catch (Exception)
                {
                    _ = 12;
                }
            }
            
            foreach (ActiveSessionGroup server in Sessions)
            {
                foreach (SessionInfoDto session in server)
                {
                    SessionInfoDto? existingSession = AllSessions.FirstOrDefault(x => x.Id == session.Id);

                    if (existingSession != null)
                    {
                        m_uiContext.Run(() => session.CopyValues(ref existingSession));

                        if (session.NowPlayingItem == null)
                        {
                            m_uiContext.Run(() => AllSessions.Remove(session));
                        }
                    }
                    else if (session.NowPlayingItem != null)
                    {
                        m_uiContext.Run(() => AllSessions.Add(session));
                    }
                }
            }

            m_uiContext.Run(() => IsRefreshing = false);
        }
        
        private async Task GetSessionsForServerAsync(JellyfinServerConnection server)
        {
            if (server.Url == null)
            {
                return;
            }
            
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(server.Url);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"MyDeviceName\", DeviceId=\"a7037817ace34ddc9d82385c63ac5f33\", Version=\"1.0.0\", Token=\"{server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            HttpResponseMessage response = await client.GetAsync("/Sessions?activeWithinSeconds=30");

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                
                SessionInfoDto[]? sessions = JArray.Parse(responseJson).ToObject<SessionInfoDto[]>();

                if (sessions != null)
                {
                    lock (Sessions)
                    {
                        ActiveSessionGroup? existing = Sessions.FirstOrDefault(x => x.Name == server.Url);
                        if (existing != null)
                        {
                            existing.Where(x => x.NowPlayingItem == null).ToList().ForEach(x =>
                            {
                                m_uiContext.Run(() => existing.Remove(x));
                            });
                            
                            foreach (SessionInfoDto session in sessions)
                            {
                                SessionInfoDto? existingSession = existing.FirstOrDefault(x => x.Id == session.Id);

                                m_uiContext.Run(() =>
                                {
                                    if (existingSession != null)
                                    {
                                        session.CopyValues(ref existingSession);
                                    }
                                    else
                                    {
                                        existing.Add(session);
                                    }
                                });
                            }
                        }
                        else
                        {
                            m_uiContext.Run(() => Sessions.Add(new ActiveSessionGroup(server.Url, sessions.Where(x => x.NowPlayingItem != null))));
                        }
                    }
                }
            }
            else
            {
                // Replace with removing server from list when multiple can be added
                await m_navigationManager.NavigateToAsync<LoginPage>();
            }
        }

        public void OnNavigatedTo()
        {
            m_uiContext.Run(() => IsRefreshing = true);
            Task.Run(RefreshTimer);
        }

        public void OnNavigatedFrom()
        {
            m_isTimerRefreshing = false;
        }
    }
}