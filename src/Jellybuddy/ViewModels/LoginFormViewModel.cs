using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Dto;
using Jellybuddy.Models;
using Jellybuddy.Pages;
using Jellybuddy.Services;
using Jellyfin.Api;
using Newtonsoft.Json.Linq;

namespace Jellybuddy.ViewModels
{
    public partial class LoginFormViewModel : ObservableObject
    {
        [ObservableProperty]
        private LoginFormModel m_model = new LoginFormModel();
        
        [ObservableProperty]
        private ICommand m_signInCommand;

        private readonly INavigationManager m_navigationManager;
        private readonly IModel<DataCache> m_dataCache;

        public LoginFormViewModel(INavigationManager navigationManager, IModel<DataCache> dataCache)
        {
            m_navigationManager = navigationManager;
            m_dataCache = dataCache;
            
            SignInCommand = new AsyncRelayCommand(OnSignIn);

            Application.Current?.Dispatcher.DispatchAsync(async () =>
            {
                IEnumerable<JellyfinServerConnection> serverConnections = await GetCachedServerConnectionsAsync();

                if (serverConnections.Any())
                {
                    m_dataCache.Data.Servers.Clear();
                    foreach (JellyfinServerConnection serverConnection in serverConnections)
                    {
                        // Probably want to reauth with the server here.
                        serverConnection.AccessToken = await AuthenticateWithServer(serverConnection);
                        
                        m_dataCache.Data.Servers.Add(serverConnection);
                    }
                    
                    // This is where we'd want to do biometric authentication
                    m_navigationManager.NavigateTo<MainTabbedPage>();
                }
            });
        }

        private async Task OnSignIn()
        {
            // Make HTTP request to server
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"MyDeviceName\", DeviceId=\"a7037817ace34ddc9d82385c63ac5f33\", Version=\"1.0.0\"");
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            
            string? accessToken = await AuthenticateWithServer(new JellyfinServerConnection()
            {
                Url = Model.ServerUrl,
                Username = Model.Username,
                Password = Model.Password
            });
            
            if (accessToken != null)
            {
                IList<JellyfinServerConnection> cachedServerConnections = (await GetCachedServerConnectionsAsync()).ToList();
                
                JellyfinServerConnection? serverConnection = cachedServerConnections.FirstOrDefault(x => x.Url == Model.ServerUrl);

                if (serverConnection == null)
                {
                    serverConnection = new JellyfinServerConnection
                    {
                        Url = Model.ServerUrl,
                        AccessToken = accessToken,
                        Username = Model.Username,
                        Password = Model.Password
                    };
                            
                    cachedServerConnections.Add(serverConnection);
                }
                else
                {
                    serverConnection.AccessToken = accessToken;
                    serverConnection.Username = Model.Username;
                    serverConnection.Password = Model.Password;
                }
                
                try
                {
                    // Cache token
                    // Securely store login details in secure storage for future use
                    string? serializedServerConnections = JsonSerializer.Serialize(cachedServerConnections);
                    await SecureStorage.Default.SetAsync("server_connections", serializedServerConnections);

                    m_dataCache.Data.Servers.Clear();
                    foreach (JellyfinServerConnection cachedServerConnection in cachedServerConnections)
                    {
                        m_dataCache.Data.Servers.Add(cachedServerConnection);
                    }
                }
                catch (Exception)
                {
                    _ = 12;
                    
                    // TODO: Probably use an alternative way to store the data.
                    // Encryption algorithm into cache folder might be okay?
                }

                m_navigationManager.NavigateTo<MainTabbedPage>();
            }
            else
            {
                // Display an error to the user if the login failed
                _ = 12;
            }
        }

        private async Task<IEnumerable<JellyfinServerConnection>> GetCachedServerConnectionsAsync()
        {
            string? storedServerConnectionsJson = null;

            try
            {
                storedServerConnectionsJson = await SecureStorage.Default.GetAsync("server_connections");
            }
            catch (Exception)
            {
                _ = 12;
                    
                // TODO: Probably use `the` alternative way to read the data.
                // Probably using an encryption algorithm from cache folder might be okay?
            }

            IEnumerable<JellyfinServerConnection> cachedServerConnections = Enumerable.Empty<JellyfinServerConnection>();
            if (storedServerConnectionsJson != null)
            {
                cachedServerConnections = JsonSerializer.Deserialize<JellyfinServerConnection[]>(storedServerConnectionsJson) ?? cachedServerConnections;
            }
            
            return cachedServerConnections;
        }

        private async Task<string?> AuthenticateWithServer(JellyfinServerConnection serverConnection)
        {
            string requestJson = JsonSerializer.Serialize(new JellyfinAuthRequest
            {
                Username = serverConnection.Username!,
                Pw = serverConnection.Password!
            });
            
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"MyDeviceName\", DeviceId=\"a7037817ace34ddc9d82385c63ac5f33\", Version=\"1.0.0\"");
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            
            string? accessToken = null;
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync($"{Model.ServerUrl}/Users/AuthenticateByName", content);

                // Get token
                if (response.IsSuccessStatusCode)
                {
                    string? responseJson = await response.Content.ReadAsStringAsync();
                    AuthenticationResult? authResult = JObject.Parse(responseJson).ToObject<AuthenticationResult>();

                    if (authResult?.User.Policy.IsAdministrator ?? false)
                    {
                        accessToken = authResult?.AccessToken;
                    }
                }
            }
            catch (Exception)
            {
                _ = 12;
            }
            
            return accessToken;
        }
    }
}