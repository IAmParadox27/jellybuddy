using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.ViewModels;

namespace Jellybuddy.Pages
{
    public partial class MainTabbedPage
    {
        private IViewModel<TabViewViewModel> m_viewModel;
        
        public MainTabbedPage(IViewModel<TabViewViewModel> tabViewViewModel)
        {
            m_viewModel = tabViewViewModel;
            BindingContext = m_viewModel.Model;
            
            InitializeComponent();
        }

        private void MainTabbedPage_OnCurrentPageChanged(object? sender, EventArgs e)
        {
            m_viewModel.Model.SelectedTab = CurrentPage;
        }
    }
}