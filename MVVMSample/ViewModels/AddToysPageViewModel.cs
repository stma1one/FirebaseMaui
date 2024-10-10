using MVVMSample.Helpers;
using MVVMSample.Models;
using MVVMSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMSample.ViewModels
{
   public  class AddToysPageViewModel : ViewModelBase
    {
        #region list of toys and toys Type will be moved to Service Folder
        List<Toy> toys = new();//רשימת הצעצועים
        //public List<ToyTypes> toyTypes
        //{
        //    get; set;
        //} = new List<ToyTypes>()
        //{
        //    new ToyTypes()
        //    {
        //        Id = 1, Name = "פאזל"
        //    },

        //    new ToyTypes()
        //    {
        //    Id = 2, Name = "משחקי חשיבה"
        //    },
        //    new ToyTypes()
        //    {
        //    Id = 3, Name = "בובה"
        //    }
        //};

        #endregion

        #region Fields
        Toy? newToy;//צעצוע להוספה

        private string? name;//שם צעצוע
        private double price;//מחיר צעצוע
        private string? errorMessage;//הודעת שגיאה
        private ToyTypes? selectedType;//סוג צעצוע נבחר

        //--------
        private IToys? toyService;//שרות לקבלת נתוני צעצועים
        private string? message;

        public string? Message
        {
            get
            {
                return message;
            }
            set
            {
                if (value != null)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }




        #endregion

        #region Properties

        //שם צעצוע
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {

                    name = value;
                    OnPropertyChanged(nameof(Name));
                    HandleError();//בדוק האם יש השם תקין
                }

            }
        }
        //האם השם של הצעצוע תקין
        public bool HasError
        {
            get
            {
                return string.IsNullOrEmpty(Name) || !ValidName();
            }

        }
        //הודעת שגיאה
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }
        
 //מחיר
        public double Price
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
                    OnPropertyChanged(nameof(Price));
                    ((Command)AddToyCommand)?.ChangeCanExecute();
                }
            }
        }
        //סוג צעצוע נבחר
        public ToyTypes? SelectedType
        {
            get
            {
                return selectedType;
            }
            set
            {
                if (selectedType != value)
                {
                    selectedType = value;
                    OnPropertyChanged(nameof(SelectedType));
                    ((Command)AddToyCommand)?.ChangeCanExecute();
                }
            }
        }

        public List<ToyTypes>? ToyTypes { get; set;}//סוגי צעצועים

        #endregion

        #region Commands
        //PROPERTY to be Bound to the Button Command
        public ICommand AddToyCommand
        {
            get; private set;
        }
        #endregion

        Task serviceCommand;

        async Task GetToysTypes()
        {
            ToyTypes = await toyService.GetToyTypes();
            OnPropertyChanged(nameof(ToyTypes));
        }
        #region Constructor
        public AddToysPageViewModel(IToys service)
        {
            //connect the property with the action method to perform
            //Command([method Name]) --> for void methods
            //Command<type>([method name]--> for methods that has parameters

            AddToyCommand = new Command<string>((async (x) => await AddnewToy(x)), (x) => CanAddToy());

            toyService=service;//ממשק לשירות צעצועים
           // serviceCommand = GetToysTypes();
            GetToysTypes().Awaiter();
           



		}

        #endregion

        #region פעולות 
        //לוגיקת שגיאה
        private void HandleError()
        {
            if (!ValidName())
            {
                ErrorMessage = "שם קצר מידי או מכיל ערכים לא תקינים";
            }
            else
                ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));
            //בדיקה האם הפעולה אפשרית
            ((Command)AddToyCommand)?.ChangeCanExecute();
        }
        
        #region Implement Valid Checks
        //1. name Should start Capital letter, at lease 4 letter word
        //Regex: ^[A-Z][a-zA-Z]{3,}
        //sample Prompt used in claude:
        /*
         * אני צריך REGEX expression עבור מחרוזת בעלת אילוצים הבאים: 
         * 1. חייבת להתחיל באות גדולה
         * 2. לפחות 4 אותיות
         * כתוב גם מקרי קצה שיש לבדוק שאכן תקין 3. 
         * 4. המלץ על אתרים בהם ניתן לבדוק אותו.
         * השימוש יעשה בשפת C# */
        private bool ValidName()
        {
            Regex reg = new Regex(@"^[A-Z][a-zA-Z]{3,}$");
            return reg.IsMatch(Name);
        }
        #endregion

        //הוספת צעצוע חדש
        private async Task AddnewToy(string name)
        {
            newToy = new Toy() {  Name = this.Name, Price = this.Price, ToyTypeKey = SelectedType.Id.ToString(), Type=SelectedType,    Image="default_image.png" };
            if (name == "הוסף צעצוע")
                newToy.IsSecondHand = false;
            else
            {
                newToy.IsSecondHand = true;
                newToy.Price = newToy.Price * 0.9;
            }
            //if (toyService.AddToy(newToy))
            //    #region שדה 
            //    Message = "הצלחה כבירה";
            //else
            //    Message = "לא הצלחתי להוסיף צעצוע";
            //#endregion

            bool res;
            #region הקפצת חלון
            try
            {
                var toy = await toyService.AddToy(newToy);
                if (toy != null)
                {
                    await Shell.Current.DisplayAlert(title: "הוספת צעצוע הצליחה", message: "הצלחה כבירה", "אישור");
                }
                else
                    res = await Shell.Current.DisplayAlert(title: "הוספת צעצוע נכשלה?", message: "?לא הצלחתי להוסיף", "אישור", "ביטול");
            }
            catch (Exception ex) { Shell.Current.DisplayAlert("Err", $"{ex.Message}", "OK"); }
            //ללא שימוש בSHELL
            // App.Current.MainPage.DisplayAlert(title: "הוספת צעצוע נכשלה?", message: "?לא הצלחתי להוסיף", "אישור", "ביטול")
                #endregion





        }
        //האם ניתן לבצע את הפעולה של הוספת צעצוע
        public bool CanAddToy()
        {
           
            //אין בעיה בשם וגם מחיר גדול מ0 וגם יש ערך בסוג צעצוע
                return !HasError && Price>0&&SelectedType!=null;
            
        }
        #endregion
    }
}
