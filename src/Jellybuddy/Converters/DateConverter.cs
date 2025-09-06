using System.Globalization;

namespace Jellybuddy.Converters
{
    public class DateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            DateTime? dateToConvert = null;
            
            if (value is DateTime date)
            {
                dateToConvert = date;
            }
            else if (value is DateTimeOffset dateTimeOffset)
            {
                dateToConvert = dateTimeOffset.DateTime;
            }

            if (dateToConvert != null)
            {
                return $"{dateToConvert.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)} {dateToConvert.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern)}";
            }
            
            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}