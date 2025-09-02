namespace Jellybuddy
{
	public partial class App
	{
		public App(IServiceProvider serviceProvider)
		{
			InitializeComponent();

			MainPage = ActivatorUtilities.CreateInstance<AppShell>(serviceProvider);
		}
	}
}
