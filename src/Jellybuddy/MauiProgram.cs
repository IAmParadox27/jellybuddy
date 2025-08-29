using System.Reflection;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Jellybuddy
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			// Pull the Syncfusion license key from the assembly metadata
			string? syncfusionLicenseKey = Assembly.GetExecutingAssembly()
				.GetCustomAttributes<AssemblyMetadataAttribute>()
				.FirstOrDefault(x => x.Key == "SyncfusionLicenseKey")?.Value;
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);
			
			MauiAppBuilder builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
