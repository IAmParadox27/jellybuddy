using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellybuddy.Core.Model;
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
        
        [ObservableProperty]
        private ICommand m_autoSignInCommand;
        
        [ObservableProperty]
        private bool m_isSheetOpen = false;

        private readonly INavigationManager<Page> m_navigationManager;
        private readonly IServerConnectionManager m_serverConnectionManager;

        public LoginFormViewModel(INavigationManager<Page> navigationManager, IServerConnectionManager serverConnectionManager)
        {
            m_navigationManager = navigationManager;
            m_serverConnectionManager = serverConnectionManager;
            
            SignInCommand = new AsyncRelayCommand(OnSignIn);
            AutoSignInCommand = new AsyncRelayCommand(OnAutoSignIn);
            
            AutoSignInCommand.Execute(null);
        }

        private async Task OnAutoSignIn()
        {
            if (m_serverConnectionManager.ActiveConnectionClient != null)
            {
                HttpResponseMessage response = await m_serverConnectionManager.ActiveConnectionClient.GetAsync("/Users/Me");
                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    JObject parseResult = JObject.Parse(json);

                    await m_navigationManager.NavigateToAsync<MainTabbedPage>();
                    return;
                }
                catch
                {
                }
            }
            
            IsSheetOpen = true;
        }

        private async Task OnSignIn()
        {
            // Make HTTP request to server
            string deviceId = Guid.NewGuid().ToString().Replace("-", "");
            
            AuthenticationResult? authResult = await AuthenticateWithServer(new JellyfinServerConnection()
            {
                Url = Model.ServerUrl,
                Username = Model.Username,
                Password = Model.Password,
                DeviceId = deviceId
            });
            
            if (authResult != null)
            {
                JellyfinServerConnection? serverConnection = new JellyfinServerConnection
                {
                    Url = Model.ServerUrl,
                    AccessToken = authResult.AccessToken,
                    Username = Model.Username,
                    Password = Model.Password,
                    DeviceId = deviceId
                };
                
                await m_serverConnectionManager.AddServerAsync(serverConnection);
                await m_serverConnectionManager.ChangeActiveConnection(serverConnection);
                
                IsSheetOpen = false;

                await m_navigationManager.NavigateToAsync<MainTabbedPage>();
            }
            else
            {
                // Display an error to the user if the login failed
                _ = 12;
            }
        }

        private async Task<AuthenticationResult?> AuthenticateWithServer(JellyfinServerConnection serverConnection)
        {
            string requestJson = JsonSerializer.Serialize(new JellyfinAuthRequest
            {
                Username = serverConnection.Username!,
                Pw = serverConnection.Password!
            });
            
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Emby-Authorization", 
                $"MediaBrowser Client=\"JellyBuddy\", Device=\"{DeviceInfo.Current.Name}\", DeviceId=\"{serverConnection.DeviceId}\", Version=\"1.0.0\"");
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            AuthenticationResult? authResult = null;
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync($"{Model.ServerUrl}/Users/AuthenticateByName", content);

                // Get token
                if (response.IsSuccessStatusCode)
                {
                    string? responseJson = await response.Content.ReadAsStringAsync();
                    authResult = JObject.Parse(responseJson).ToObject<AuthenticationResult>();

                    if (!(authResult?.User.Policy.IsAdministrator ?? false))
                    {
                        authResult = null;
                    }
                }
            }
            catch (Exception)
            {
                _ = 12;
            }
            
            return authResult;
        }
    }
}