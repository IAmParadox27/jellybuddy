using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jellybuddy.Core.Library;
using Jellybuddy.Core.Model;
using Jellybuddy.Dto;
using Jellybuddy.Models;

namespace Jellybuddy.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, IPageViewModel
    {
        [ObservableProperty]
        private JellyfinServerConnection? m_selectedServer;
        
        [ObservableProperty]
        private IModel<DataCache> m_dataCache;
        
        [ObservableProperty]
        private bool m_bottomSheetOpen = false;
        
        [ObservableProperty]
        private ICommand m_openAddServerBottomSheetCommand;

        [ObservableProperty]
        private ICommand m_addServerCommand;
        
        [ObservableProperty]
        private JellyfinServerConnection m_newServer = new JellyfinServerConnection();

        [ObservableProperty]
        private IServerConnectionManager m_serverConnectionManager;

        public SettingsViewModel(IServerConnectionManager serverConnectionManager)
        {
            ServerConnectionManager = serverConnectionManager;
            
            OpenAddServerBottomSheetCommand = new RelayCommand(OnOpenAddServerBottomSheet);
            AddServerCommand = new AsyncRelayCommand(OnAddServerCommand);
        }

        private async Task OnAddServerCommand()
        {
            // Temp code for testing
            await ServerConnectionManager.AddServerAsync(NewServer);
            NewServer = new JellyfinServerConnection();
            
            BottomSheetOpen = false;
        }

        private void OnOpenAddServerBottomSheet()
        {
            BottomSheetOpen = true;
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }
    }
}