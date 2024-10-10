using MVVMSample.ViewModels;
namespace MVVMSample.Views;

public partial class ToyDetailsPage : ContentPage
{
	public ToyDetailsPage(ToyDetailsPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm ;
	}

}