using Firebase.Auth;
using Firebase.Database.Streaming;
using Microsoft.VisualBasic;
using MVVMSample.Helpers;
using MVVMSample.Models;
using MVVMSample.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace MVVMSample.ViewModels
{
    public class ViewToysPageViewModel : ViewModelBase
    {
        #region Fields
        private double? price;
        private ObservableCollection<Toy> toys;
        private IToys toyService;
        private ObservableCollection<Toy> fullList = new ObservableCollection<Toy>();
        private bool isRefreshing;

        #region load ToyTypes
        List<ToyTypes> toyTypes;
        #endregion

        #region observable
        IDisposable dispose;
        #endregion

        #region נבחר מהרשימה
        private Toy selectedToy;
        private bool hasInternet;

        #region Because of Online- are we in filter Mode?
        private bool isFilterAbove;
        private bool isFilterBelow;

        #endregion

        public Toy SelectedToy
        {
            get
            {
                return selectedToy;
            }
            set
            {
                if (selectedToy != value)
                {
                    selectedToy = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region בחירת אוסף פריטים מהרישמה
        public ObservableCollection<object> SelectedToys
        {
            get; set;
        }

        #endregion


        #endregion

        #region Properties


        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Toy> Toys
        {
            get => toys;
            set
            {
                if (toys != value)
                {
                    toys = value;
                    OnPropertyChanged();
                }
            }
        }
        public double? Price
        {
            get
            {
                return price;
            }
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged();
                    RefreshCommands();
                }
            }
        }

        #endregion

        #region COMMANDS

        public ICommand DeleteCommand
        {
            get; private set;
        }
        public ICommand RefreshCommand
        {
            get; private set;
        }
        public ICommand FilterAbovePriceCommand
        {
            get; private set;
        }

        public ICommand FilterBelowPriceCommand
        {
            get; private set;
        }

	 		#region Navigation
		public ICommand ShowDetailsCommand
        {
            get;private set;
        }
        //Shell Navigation Pass Arguments


        //Shell Navigation Pass Object

        #endregion

        #endregion

        #region Constructor
        public ViewToysPageViewModel(IToys service)
        {
            //Notify when device goes off internet or back on
            hasInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
            Connectivity.ConnectivityChanged +=(async(s,e)=>await NotifyWhenInternetIsLost(s,e));
            #region Init Data
            isFilterAbove = false;
            isFilterBelow = false;
            Price = null;
            Toys=new ObservableCollection<Toy>();
            toyService=service;
            //LoadToysType().Awaiter(completedCallback:LoadData  ,failedCallBack: HandleError);
            //LoadData().Awaiter(failedCallBack:HandleError);
           // Refresh().Awaiter();
         //   toys=new ObservableCollection<Toy>(toyService.GetToys());
           
           
           
            #endregion


            #region Init Commands
            FilterAbovePriceCommand = new Command(execute:FilterAbove,()=>Toys!=null&&Toys.Count>0);
            FilterBelowPriceCommand = new Command(FilterBelow,()=>Price>0);
            RefreshCommand = new Command(async()=>await Refresh());
            DeleteCommand = new Command<Toy>(async(t) => { await toyService.DeleteToy(t); });

            #region Navigation Commands
            //Navigation with Parametes
            // ShowDetailsCommand = new Command(async() => { await GotoWithArguments(); });

            //Navigation With Object
            ShowDetailsCommand = new Command(async() => { await GoToDetailsPage(); });
            #endregion

            #region Commands By LINQ
            //FilterAbovePriceCommand = new Command(() => Toys = new ObservableCollection<Toy>(Toys.Where(t => t.Price > Price)));
            //FilterBelowPriceCommand = new Command(() => {
            //    var toys = Toys.Where(t => t.Price > Price);
            //    foreach (var toy in toys)
            //    {
            //       Toys.Remove(toy);
            //    }
            //});
            #endregion

            #endregion

        }

		private void HandleError(Exception ex)
		{
            Shell.Current.DisplayAlert($"oopsi", $"doopsie:{ex.Message}", "OK");
		}




		#endregion

		#region Methods
		private async Task Refresh()
        {
           
            IsRefreshing = true;
            await Task.Delay(2000);
                if (hasInternet)
                {
                    //fullList = await toyService.GetToys();


                    Toys.Clear();
                    UnloadData();
				await Task.Delay(200);
				    LoadData();
                    Price = null;
                    RefreshCommands();
                }
                IsRefreshing = false;
                isFilterAbove = false;
                isFilterBelow = false;
            
        }
        private void FilterAbove()
        {
            isFilterAbove = true;
            isFilterBelow = false;
            //כל הצעצועים שגדולים מהמחיר
            var toys =Toys.Where(t => t.Price > Price).ToList();
            Toys.Clear();
            foreach (var t in toys)
                Toys.Add(t);
            RefreshCommands();
            
        } private void FilterBelow()
        {
            isFilterBelow = true;
            isFilterAbove = false;
            var toys = fullList?.Where(t => t.Price <= Price);
           if(Toys!=null&& Toys.Count>0) 
            Toys.Clear();
            if(toys!=null&&Toys!=null)
            foreach (var t in toys)
                Toys.Add(t);
            RefreshCommands();

        }

        private void RefreshCommands()
        {
            var filterabove = FilterAbovePriceCommand as Command;
            if (filterabove != null)
            {
                filterabove.ChangeCanExecute();

            }
            var filterbelow = FilterBelowPriceCommand as Command;

            filterbelow?.ChangeCanExecute();

            

        }
        #region Navigation Methods
        //Navigation with Object
        private async Task GoToDetailsPage()
        {
            if (SelectedToy == null)
                return;
            //האובייקטים שנרצה להעביר יישמרו במילון
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Toy", SelectedToy);
            //נשלח את המידע עם הפניה למסך
            await Shell.Current.GoToAsync("/Details", data);
            //נבטל את הבחירה בחזרה למסך הקודם
            SelectedToy = null;
            

           
        }
        //Navigation With Parameters
        private async Task GotoWithArguments()
        {
            if (SelectedToy != null)
            {
                await Shell.Current.GoToAsync($"/Details?id={SelectedToy.Id}");

                SelectedToy = null;
            }
            
        }
        #endregion
        async Task GetToysAsync()
        {
           var list = await toyService.GetToys();
            fullList.Clear();
            foreach (var x in list)
                fullList.Add(x);
            Toys = new ObservableCollection<Toy>();
            if(fullList != null)
            foreach (var toy in fullList)
                Toys.Add(toy);
        }


        //כאשר האינטרנט הולך פייפן נרצה להודיע למשתמש וגם לא לאפשר להפעיל ממשקים מבוססי אינטרנט
        async Task NotifyWhenInternetIsLost(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No Internet", "Connection lost", "Ok");
                hasInternet = false;
            }
            else
            {
				await Shell.Current.DisplayAlert(" Internet BACK!", "Connection Restored", "Ok");
				hasInternet = true;
            }
            //check Wifi
			IEnumerable<ConnectionProfile> profiles = Connectivity.Current.ConnectionProfiles;

			if (profiles.Contains(ConnectionProfile.WiFi))
			{
				// Active Wi-Fi connection.
				await Shell.Current.DisplayAlert("YEAH WIFI", "You have WIFI", "Ok");
			}
            else
				await Shell.Current.DisplayAlert(" NO WIFI", "באסה", "Ok");

		}
		private void RemoveToy(string? firebaseKey)
		{

            var item =Toys?.Where(x => x.FirebaseKey == firebaseKey).FirstOrDefault();
				Toys.Remove(item);

			
		}
        public async Task LoadToysType()
        {


            toyTypes = await toyService?.GetToyTypes();


        }
        public void UnloadData()
        {

            if (dispose != null)
            {
                dispose.Dispose();
                dispose = null;
            }
        }
        //public async Task LoadData()
        public async void LoadData()
        {

            if (dispose == null)
                try
                {
                   
                    if (toyService is RealTimeService)
                        dispose = (toyService as RealTimeService)
                            .RegisterToys()
                            .Subscribe(item =>
                            {
                                try
                                {
                                    if (item != null && item.Object != null)
                                    {

                                        switch (item.EventType)
                                        {

                                            case FirebaseEventType.Delete:
                                                {
                                                    RemoveToy(item.Key);
                                                    break;
                                                }
                                            case FirebaseEventType.InsertOrUpdate:
                                                {

                                                    AddOrUpdateToy(item);


                                                    break;
                                                }
                                            default:
                                                break;
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {

                                    throw new Exception(ex.Message, ex);

                                }


                            }, onError: HandleError);

                }
                catch (Exception ex) { IsRefreshing = false; throw new Exception(ex.Message, ex); }

        }
    

        private void AddOrUpdateToy(FirebaseEvent<Toy> item)
        {
            Toy toy;
	    //האם צעצוע חדש או קיים
            var existingToy = Toys.Where(x => x.FirebaseKey == item.Key).ToList();

	    //מכיוון שעדכון של צעצוע לא יעדכן את המסך
     	    //ניצור צעצוע חדש בכל מקרה
     
            toy = new Toy()
            {
                FirebaseKey = item.Key,
                Price = item.Object.Price,
                IsSecondHand = item.Object.IsSecondHand,
                Image = item.Object.Image,
                Name = item.Object.Name,
                ToyTypeKey = item.Object.ToyTypeKey,
                //load Toy type
                Type = toyTypes.Where(x => x.Id == int.Parse(item.Object.ToyTypeKey)).FirstOrDefault()
			};

     //אם אנחנו במצב של פילטור- צריך להחליט האם צריך להוסיף אותו לאוסף המוצג או לא
            
       bool applyFilter=(!isFilterAbove && !isFilterBelow) ||(isFilterAbove && Price<toy.Price)||(isFilterBelow && Price>=toy.Price);
            //אם זה צעצוע קיים ואין פילטר או שהצעצוע עומד בתנאי הפילטור
            if (existingToy.Count > 0 && applyFilter)
            {
                foreach (var t in existingToy)
                {
                    //נמצא את המיקום שלו - ונכניס את הצעצוע החדש במקומו
                    int index = Toys.IndexOf(t);
                    Toys?.RemoveAt(index);
                    Toys?.Insert(index, toy);
                }
            }
            //אם זה צעצוע חדש
            else if (applyFilter)
                Toys?.Add(toy);
            //אם צעצוע קיים אבל לא עומד בתנאי הפילטור 
            else
                foreach (var t in existingToy)
                {
                    Toys.Remove(t);

                }           
        
        }
        #endregion

        //remove toy from list
    }
}
