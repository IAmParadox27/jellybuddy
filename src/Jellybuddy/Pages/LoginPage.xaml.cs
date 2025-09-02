using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Services;
using Jellybuddy.ViewModels;

namespace Jellybuddy
{
	public partial class LoginPage : IViewModelPage<LoginFormViewModel>
	{
		public LoginPage()
		{
			InitializeComponent();
		}
	}
}
