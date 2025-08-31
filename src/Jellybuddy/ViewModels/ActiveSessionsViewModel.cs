using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Dto;
using Jellybuddy.Models;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class ActiveSessionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ActiveSessionGroup> m_sessions = new ObservableCollection<ActiveSessionGroup>();
        
        public ActiveSessionsViewModel(IModel<DataCache> model)
        {
            model.Data.Servers.ToList().ForEach(async void (x) =>
            {
                try
                {
                    await GetSessionsForServerAsync(x);
                }
                catch (Exception e)
                {
                    _ = 12;
                }
            });
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
                        Sessions.Add(new ActiveSessionGroup(server.Url, sessions));
                    }
                }
            }
        }
    }
}