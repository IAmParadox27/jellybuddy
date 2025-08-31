using System.Globalization;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    public class SessionToProgressConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SessionInfoDto session)
            {
                return Math.Round((double)(session.PlayState.PositionTicks ?? 0) / (double)(session.NowPlayingItem.RunTimeTicks ?? 1), 2);
            }
            
            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}