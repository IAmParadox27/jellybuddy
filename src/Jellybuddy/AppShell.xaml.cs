using Jellybuddy.Pages;

namespace Jellybuddy
{
	public partial class AppShell
	{
		public AppShell()
		{
			InitializeComponent();
			
			Routing.RegisterRoute(nameof(MainTabbedPage), typeof(MainTabbedPage));
		}
	}
}
