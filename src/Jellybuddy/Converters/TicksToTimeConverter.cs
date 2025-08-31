using System.Globalization;

namespace Jellybuddy.Converters
{
    public class TicksToTimeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is long ticks)
            {
                TimeSpan timeSpan = new TimeSpan(ticks);
                
                string result;

                if (timeSpan.Hours > 0)
                {
                    result = $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                }
                else
                {
                    result = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
                }
                
                return result;
            }

            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}