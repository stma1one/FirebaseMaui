using Microsoft.Maui.ApplicationModel;
using MVVMSample.Models;
using MVVMSample.Services;
using MVVMSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.Json;

namespace MVVMSample.ViewModels
{
    public class LoginPageViewModel: ViewModelBase
    {
        #region Fields
        private IToys service;//service
        #endregion

        #region Properties
        #endregion

        #region Commands
        public ICommand LoginCommand { get; set; }
        public ICommand GoToWorkCommand
        {
            get; private set;   
        }
        
        #endregion
        public LoginPageViewModel(IToys service)
        {
            this.service = service;
            InServerCall = false;
            
            this.LoginCommand = new Command(OnLogin);
            GoToWorkCommand = new Command(async () => await NavigateToWork());
            
            
        }

		private async Task NavigateToWork()
		{
			bool supportsUri = await Launcher.Default.CanOpenAsync("waze://");

			if (supportsUri)
				await Launcher.Default.OpenAsync("waze://?favorite=work&navigate=yes");
		}

		private async void OnLogin()
        {
            //Choose the way you want to blobk the page while indicating a server call
            InServerCall = true;
            

          var user= await this.service.Login(Email,Password);
            
            InServerCall = false;

            //Set the application logged in user to be whatever user returned (null or real user)
           
            if (user == null)
            {

                //await Application.Current.MainPage.DisplayAlert("Login", "Login Faild!", "ok");
                await Shell.Current.DisplayAlert("Login", "Login Faild!", "ok");
            }
            else
            {
                //await Application.Current.MainPage.DisplayAlert("Login", $"Login Succeed! for {user.Name}", "ok");
                await Shell.Current.DisplayAlert("Login", $"Login Succeed! for {user.Name}", "ok");
                App.user = user;
                
                    await Shell.Current.GoToAsync("///MainPage");
				    



			}
        }

       

        private bool inServerCall;
        public bool InServerCall
        {
            get
            {
                return this.inServerCall;
            }
            set
            {
                this.inServerCall = value;
                OnPropertyChanged("NotInServerCall");
                OnPropertyChanged("InServerCall");
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password= value;
                OnPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
                OnPropertyChanged();
            }
        }
        public bool NotInServerCall
        {
            get
            {
                return !this.InServerCall;
            }
        }
    }
}
