using System.Globalization;
using System.Reflection;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    public class SessionToItemPosterConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SessionInfoDto session)
            {
                Guid itemPostedId = session.NowPlayingItem.Type == BaseItemDtoType.Movie ?
                    session.NowPlayingItem.Id :
                    session.NowPlayingItem.SeriesId ?? session.NowPlayingItem.ParentId ?? session.NowPlayingItem.Id;

                string? jellyfinServerUrl = typeof(IModel<>).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .FirstOrDefault(x => x.Key == "JellyfinServerUrl")?.Value;
                
                return $"{jellyfinServerUrl}/Items/{itemPostedId}/Images/Primary?width=288&height=408";
            }
            
            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}