using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellybuddy.Services;
using Jellybuddy.ViewModels;
using Syncfusion.Maui.Toolkit.TabView;

namespace Jellybuddy.Pages
{
    public partial class MainTabbedPage : IViewModelPage<TabViewViewModel>
    {
        public TabViewViewModel? ViewModel => (TabViewViewModel?)BindingContext;
        
        public MainTabbedPage()
        {
            InitializeComponent();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            
            if (ViewModel != null)
            {
                ViewModel.SelectedTab = TabView.Items.First().Content;
            }
        }

        private void TabView_OnSelectionChanged(object? sender, TabSelectionChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.SelectedTab = TabView.Items[e.NewIndex].Content;
            }
        }
    }
}