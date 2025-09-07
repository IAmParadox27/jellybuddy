using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jellyfin.Api;

namespace Jellybuddy.Controls
{
    public partial class UserControl
    {
        public static readonly BindableProperty ItemCountsProperty = BindableProperty.Create(
            nameof(ItemCounts), typeof(ItemCounts), typeof(UserControl));

        public ItemCounts ItemCounts
        {
            get => (ItemCounts)GetValue(ItemCountsProperty);
            set => SetValue(ItemCountsProperty, value);
        }
        
        public UserControl()
        {
            InitializeComponent();
        }
    }
}