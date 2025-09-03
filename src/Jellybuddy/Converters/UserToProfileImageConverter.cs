using System.Globalization;
using System.Reflection;
using Jellybuddy.Core.Library;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters;

public class UserToProfileImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // TODO: Change this to be passed in from the converter paramter because this isn't fully valid.
        string? jellyfinServerUrl = typeof(IModel<>).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(x => x.Key == "JellyfinServerUrl")?.Value;
        
        if (value is UserDto user)
        {
            return $"{jellyfinServerUrl}/Users/{user.Id}/Images/Primary?quality=90";
        }
        else if (value is Guid userId)
        {
            return $"{jellyfinServerUrl}/Users/{userId}/Images/Primary?quality=90";
        }
        
        return Binding.DoNothing;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}