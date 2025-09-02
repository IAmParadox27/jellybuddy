using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Services;
using Jellybuddy.ViewModels;
using Syncfusion.Maui.TabView;

namespace Jellybuddy.Pages
{
    public partial class MainTabbedPage : IViewModelPage<TabViewViewModel>
    {
        public TabViewViewModel? ViewModel => (TabViewViewModel?)BindingContext;
        
        public MainTabbedPage()
        {
            InitializeComponent();
        }

        private void MainTabbedPage_OnCurrentPageChanged(object? sender, EventArgs e)
        {
        }

        private void SfTabView_OnSelectionChanged(object? sender, TabSelectionChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.SelectedTab = TabView.Items[(int)e.NewIndex].Content;
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            if (ViewModel != null)
            {
                ViewModel.SelectedTab = TabView.Items.First().Content;
            }
        }
    }
}