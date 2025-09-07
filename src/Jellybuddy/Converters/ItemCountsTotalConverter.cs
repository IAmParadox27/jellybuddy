using System.Globalization;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    // For now we're just going to make this count Episodes + Movies but we might change this to be more dynamic at a later stage
    public class ItemCountsTotalConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ItemCounts itemCounts)
            {
                return itemCounts.EpisodeCount + itemCounts.MovieCount;
            }

            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}