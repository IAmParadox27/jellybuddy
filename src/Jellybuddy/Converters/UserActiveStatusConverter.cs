using System.Globalization;
using Jellybuddy.ViewModels;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    public enum UserActiveStatus
    {
        Active,
        Idle,
        Offline,
    }
    
    public class UserActiveStatusConverter : IValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int requiredValues = 2;

            if (values.Length != requiredValues)
            {
                return Binding.DoNothing;
            }
            
            if (values[0] is UserDto user &&
                values[1] is IEnumerable<SessionInfoDto> sessions)
            {
                
            }
            
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is UserEntryViewModel userEntryViewModel)
            {
                // TODO: Move the magic 5 number into an app setting that the user can configure what is considered "active"
                if ((userEntryViewModel.User?.LastActivityDate.HasValue ?? false) && userEntryViewModel.User.LastActivityDate.Value.AddMinutes(5) > DateTime.Now)
                {
                    return UserActiveStatus.Active;
                }

                IEnumerable<SessionInfoDto> userSessions = userEntryViewModel.UserSessions.Where(x => x.UserId == userEntryViewModel.User!.Id);

                if (userSessions.Any(x => !x.PlayState.IsPaused))
                {
                    return UserActiveStatus.Active;
                }

                if (userSessions.Any() && userSessions.All(x => x.PlayState.IsPaused))
                {
                    return UserActiveStatus.Idle;
                }
                
                // TODO: Move the magic 30 number into an app setting that the user can configure what is considered "idle"
                if (userEntryViewModel.User!.LastActivityDate.HasValue && userEntryViewModel.User.LastActivityDate.Value.AddMinutes(30) > DateTime.Now)
                {
                    return UserActiveStatus.Idle;
                }
                
                return UserActiveStatus.Offline;
            }
            
            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}