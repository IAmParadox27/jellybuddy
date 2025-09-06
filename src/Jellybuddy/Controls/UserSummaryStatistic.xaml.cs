using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellybuddy.Controls
{
    public partial class UserSummaryStatistic : ContentView
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text), typeof(string), typeof(UserSummaryStatistic));

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            nameof(Value), typeof(object), typeof(UserSummaryStatistic));
        
        public static readonly BindableProperty IconProperty = BindableProperty.Create(
            nameof(Icon), typeof(ImageSource), typeof(UserSummaryStatistic));
        
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        
        public object Value
        {
            get => (object)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        
        public UserSummaryStatistic()
        {
            InitializeComponent();
        }
    }
}