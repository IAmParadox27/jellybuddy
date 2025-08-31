using Jellybuddy.Core.DependencyInjection;
using Jellybuddy.ViewModels;

namespace Jellybuddy
{
	public partial class LoginPage
	{
		public LoginPage(IViewModel<LoginFormViewModel> viewModel)
		{
			BindingContext = viewModel.Model;
			
			InitializeComponent();
		}
	}
}
