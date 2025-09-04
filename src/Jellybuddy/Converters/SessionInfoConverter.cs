using System.Globalization;
using Jellyfin.Api;
using Type = System.Type;

namespace Jellybuddy.Converters
{
    public enum SessionInfoType
    {
        AudioCodec,
        VideoCodec,
        Resolution,
        VideoFormat
    }
    
    public class SessionInfoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SessionInfoDto session && parameter is SessionInfoType type)
            {
                switch (type)
                {
                    case SessionInfoType.AudioCodec:
                        return session.NowPlayingItem.MediaStreams
                            .FirstOrDefault(x => x.Type == MediaStreamType2.Audio)?.Codec.ToUpper() ?? Binding.DoNothing;
                    case SessionInfoType.VideoCodec:
                        return session.NowPlayingItem.MediaStreams
                            .FirstOrDefault(x => x.Type == MediaStreamType2.Video)?.Codec.ToUpper() ?? Binding.DoNothing;
                    case SessionInfoType.Resolution:
                    {
                        MediaStream? videoStream = session.NowPlayingItem.MediaStreams
                            .FirstOrDefault(x => x.Type == MediaStreamType2.Video);

                        if (videoStream != null)
                        {
                            return videoStream.DisplayTitle.ToLower()
                                .Replace(videoStream.Codec.ToLower(), string.Empty)
                                .Replace(string.IsNullOrEmpty(videoStream.VideoDoViTitle) 
                                    ? videoStream.VideoRange.ToString().ToLower() 
                                    : videoStream.VideoDoViTitle.ToLower(), string.Empty)
                                .Trim()
                                .ToUpper();
                        }
                        break;
                    }
                    case SessionInfoType.VideoFormat:
                        return session.NowPlayingItem.Container.Split(',').First().ToUpper();
                }
            }

            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}