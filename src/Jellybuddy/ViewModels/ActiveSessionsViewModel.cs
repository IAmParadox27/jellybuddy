using System.Collections.ObjectModel;
using System.Text.Json;
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
    public partial class ActiveSessionsViewModel : ObservableObject
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

        public ActiveSessionsViewModel(IModel<DataCache> model, INavigationManager navigationManager)
        {
            m_model = model;
            m_navigationManager = navigationManager;

            RefreshCommand = new AsyncRelayCommand(LoadSessionsAsync);
            
            Task.Run(LoadSessionsAsync);
            Task.Run(RefreshTimer);
        }

        private async Task RefreshTimer()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                
                await LoadSessionsAsync(false);
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
                IsRefreshing = true;
            }
            
            foreach (JellyfinServerConnection serverConnection in m_model.Data.Servers)
            {
                try
                {
                    await GetSessionsForServerAsync(serverConnection);
                }
                catch (Exception e)
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
                        session.CopyValues(ref existingSession);

                        if (session.NowPlayingItem == null)
                        {
                            AllSessions.Remove(session);
                        }
                    }
                    else if (session.NowPlayingItem != null)
                    {
                        AllSessions.Add(session);
                    }
                }
            }

            if (isRefreshing)
            {
                IsRefreshing = false;
            }
        }
        
        private async Task GetSessionsForServerAsync(JellyfinServerConnection server)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(server.Url);
            client.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"MyDeviceName\", DeviceId=\"a7037817ace34ddc9d82385c63ac5f33\", Version=\"1.0.0\", Token=\"{server.AccessToken}\"");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            HttpResponseMessage response = await client.GetAsync("/Sessions?activeWithinSeconds=30");

            if (response.IsSuccessStatusCode)
            {
                string? responseJson = await response.Content.ReadAsStringAsync();
                
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
                                existing.Remove(x);
                            });
                            
                            foreach (SessionInfoDto session in sessions)
                            {
                                SessionInfoDto? existingSession = existing.FirstOrDefault(x => x.Id == session.Id);

                                if (existingSession != null)
                                {
                                    session.CopyValues(ref existingSession);
                                }
                                else
                                {
                                    existing.Add(session);
                                }
                            }
                        }
                        else
                        {
                            Sessions.Add(new ActiveSessionGroup(server.Url, sessions.Where(x => x.NowPlayingItem != null)));
                        }
                    }
                }
            }
            else
            {
                // Replace with removing server from list when multiple can be added
                m_navigationManager.NavigateTo<LoginPage>();
            }
        }
    }
}