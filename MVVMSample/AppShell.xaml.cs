using MVVMSample.Models;
using MVVMSample.Views;
using System.Windows.Input;

namespace MVVMSample
{
    public partial class AppShell : Shell
    {

        public static User? user;
        public ICommand LogoutCommand => 
            new Command(async ()=> {
                App.user = null;
                if (Preferences.Default.ContainsKey("userObj"))
                    Preferences.Default.Remove("userObj");
                await Shell.Current.DisplayAlert("Goodbye", "שלום", "אישור");
               
                await Shell.Current.GoToAsync("Login");
                Shell.Current.FlyoutIsPresented = false;

		});
        //{
        //    get; private set;   
        //}
        public AppShell()
        {
            
            InitializeComponent();
            BindingContext=this;    
            #region רישום מסכים פנימיים
            Routing.RegisterRoute("Details", typeof(ToyDetailsPage));
            Routing.RegisterRoute("Login",typeof(LoginPage));   
           
            #endregion
        }

		



	}
}
