using System.Globalization;
using Jellybuddy.ViewModels;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    public class UserTabToItemCountsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (values[0] is UsersViewModel usersViewModel && values[1] is UserDto user)
                {
                    return usersViewModel.ItemCounts.FirstOrDefault(x => x.UserId == user.Id).Counts;
                }
            }
            
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}