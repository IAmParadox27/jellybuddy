using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Jellybuddy.Core.Library;
using Jellybuddy.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.Services
{
    public partial class ServerConnectionManager : ObservableObject, IServerConnectionManager
    {
        [ObservableProperty]
        private JellyfinServerConnection? m_activeConnection;

        private RangedObservableCollection<JellyfinServerConnection> m_servers = new RangedObservableCollection<JellyfinServerConnection>();

        public IList<JellyfinServerConnection> Servers => m_servers;
        
        public HttpClient? ActiveConnectionClient { get; private set; }

        public ServerConnectionManager()
        {
            string? serverUrl = SecureStorage.GetAsync("default_server_url").GetAwaiter().GetResult();
            string? deviceId = SecureStorage.GetAsync("default_server_device_id").GetAwaiter().GetResult();
            string? token = SecureStorage.GetAsync("default_server_token").GetAwaiter().GetResult();

            if (serverUrl != null && deviceId != null && token != null)
            {
                ChangeActiveConnection(new JellyfinServerConnection()
                {
                    Url = serverUrl,
                    DeviceId = deviceId,
                    AccessToken = token
                }).GetAwaiter().GetResult();
            }
            
            string? serversJson = SecureStorage.GetAsync("servers").GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(serversJson))
            {
                JArray servers = JArray.Parse(serversJson);
                
                m_servers.AddRange(servers.ToObject<JellyfinServerConnection[]>());
            }
            else if (ActiveConnection != null)
            {
                _ = AddServerAsync(ActiveConnection);
            }
        }
        
        public async Task ChangeActiveConnection(JellyfinServerConnection connection)
        {
            // Set the active connection
            ActiveConnection = connection;
        }

        public async Task AddServerAsync(JellyfinServerConnection server)
        {
            Servers.Add(server);
            
            JArray servers = new JArray();
            try
            {
                string? serversJson = await SecureStorage.GetAsync("servers");

                if (!string.IsNullOrEmpty(serversJson))
                {
                    servers = JArray.Parse(serversJson);
                }
            }
            catch (Exception e)
            {
                _ = 12;
            }
            finally
            {
                servers.Add(new JObject
                {
                    ["Url"] = server.Url,
                    ["DeviceId"] = server.DeviceId,
                    ["AccessToken"] = server.AccessToken
                });
            
                await SecureStorage.SetAsync("servers", servers.ToString(Formatting.None));
            }
        }

        public async Task RemoveServer(JellyfinServerConnection server)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Servers.Remove(server);
            });
            
            string? serversJson = await SecureStorage.GetAsync("servers");

            if (!string.IsNullOrEmpty(serversJson))
            {
                JArray servers = JArray.Parse(serversJson);
                
                JToken? existingServer = servers.FirstOrDefault(x =>
                {
                    if (x is JObject obj)
                    {
                        return obj["url"]?.ToString() == server.Url;
                    }

                    return false;
                });

                if (existingServer != null)
                {
                    servers.Remove(existingServer);
                    
                    await SecureStorage.SetAsync("servers", servers.ToString(Formatting.None));
                }
            }
        }

        partial void OnActiveConnectionChanged(JellyfinServerConnection? oldConnection, JellyfinServerConnection? connection)
        {
            if (connection != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // Save this in the secure storage for later use
                        await SecureStorage.SetAsync("default_server_url", connection.Url!);
                        await SecureStorage.SetAsync("default_server_device_id", connection.DeviceId);
                        await SecureStorage.SetAsync("default_server_token", connection.AccessToken!);

                        // Configure the client to be setup for this connection
                        ActiveConnectionClient = new HttpClient();
                        ActiveConnectionClient.BaseAddress = new Uri(connection.Url!);
                        ActiveConnectionClient.DefaultRequestHeaders.Add("X-Emby-Authorization",
                            $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{connection.DeviceId}\", Version=\"1.0.0\", Token=\"{connection.AccessToken}\"");
                        ActiveConnectionClient.DefaultRequestHeaders.Add("Accept", "*/*");
                    }
                    catch (Exception e)
                    {
                        await RemoveServer(connection);
                        ActiveConnection = oldConnection;
                    }
                }).GetAwaiter().GetResult();
            }
        }
    }
}