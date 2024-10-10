using MVVMSample.ViewModels;

namespace MVVMSample.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}