using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Jellybuddy
{
	[Application(UsesCleartextTraffic = true)]
	public class MainApplication : MauiApplication
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownership)
			: base(handle, ownership)
		{
			Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
			{
				if (view is Entry)
				{
					// Remove underline
					handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
				}
			});
		}

		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	}
}
