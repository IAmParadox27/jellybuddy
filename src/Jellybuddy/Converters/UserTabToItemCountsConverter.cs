using System.Globalization;
using Jellybuddy.ViewModels;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    public class UserTabToItemCountsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is UserEntryViewModel userEntryViewModel)
            {
                return userEntryViewModel.ItemCounts;
            }
            
            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}