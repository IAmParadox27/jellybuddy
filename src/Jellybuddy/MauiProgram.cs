using System.Reflection;
using CommunityToolkit.Maui;
using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Services;
using Maui.Biometric;
using MauiIcons.Core;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

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
				.UseBiometricAuthentication()
				.UseMauiIconsCore()
				.UseMaterialMauiIcons()
				.UseViewModels()
				.ConfigureSyncfusionCore()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			builder.Services.AddSingleton<INavigationManager, NavigationManager>();
			builder.Services.AddSingleton<IUIContext, UIContext>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
