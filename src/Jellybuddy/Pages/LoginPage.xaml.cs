using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.Core.Library;
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
