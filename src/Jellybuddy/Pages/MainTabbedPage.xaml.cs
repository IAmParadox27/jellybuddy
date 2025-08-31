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
        public IViewModel<ActiveSessionsViewModel> ActiveSessionsViewModel { get; }
        
        public MainTabbedPage(IViewModel<ActiveSessionsViewModel> activeSessionsViewModel)
        {
            BindingContext = this;
            ActiveSessionsViewModel = activeSessionsViewModel;
            
            InitializeComponent();
        }
    }
}