using MVVMSample.Models;
using MVVMSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMSample.ViewModels
{
    #region פרמטרים ממסכים אחרים
    //פרמטר ראשון - השם של התכונה במסך החדש
    //פרמטר שני- שם של המפתח במילון או השם של הפרמטר במחרוזת 

    [QueryProperty(nameof(SelectedToy), "Toy")]
    [QueryProperty(nameof(Id), "id")]
    #endregion
    public class ToyDetailsPageViewModel : ViewModelBase
    {
        private IToys toyService;
        private int id;
        private Toy? selectedToy;

        /*add support for changing photo
         *add Property that updates and reads the SelectedToy.Image
         *
         *
         */
        public string SelectedImage { get => SelectedToy?.Image; set { if (value != SelectedToy?.Image) { SelectedToy.Image = value; OnPropertyChanged(); } } }
        public ICommand ChangePhotoCommand { get; private set;
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();

                    //Fetch the Toy by Id
                    FetchToyById();
                }


            }
        }
        /*
         * add Command for Uploading Image
         * 
         */

        private async void FetchToyById()
        {
            SelectedToy = (await toyService?.GetToys())?.Where(t => t.Id == id).FirstOrDefault();
        }

        public Toy? SelectedToy
        {
            get => selectedToy;
            set
            {
                if (selectedToy != value)
                {
                    selectedToy = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SecondHandStatus));
                    OnPropertyChanged(nameof(SelectedImage));
                }
            }
        }



        public string SecondHandStatus => SelectedToy?.IsSecondHand == true ? "Condition: Second Hand" : "Condition: New";

        public ToyDetailsPageViewModel(IToys service)
        {
            toyService = service;
            //Add Command for uploading image   
            ChangePhotoCommand = new Command(async () => await ChangeImage());
        }

        private async Task ChangeImage()
        {
            var backup = SelectedImage;
            SelectedImage = "loadingforever.gif";
            string choice = await Shell.Current.DisplayActionSheet("בחרו פעולה", cancel: "ביטול", null, "צלם", "בחר תמונה");
                
            FileResult photo=null;

            try
            {
                MediaPickerOptions options = new MediaPickerOptions() { Title = "Smile" };
                switch (choice)
                {
                    case "צלם":
                        if (MediaPicker.Default.IsCaptureSupported)
                        {

                            photo = await MediaPicker.Default.CapturePhotoAsync(options);
                        }

                        break;
                    case "בחר תמונה":
                        photo=await MediaPicker.Default.PickPhotoAsync(options);
                        break;
                    default:
                        SelectedImage = backup;

						break;
                }
                //Add Method Upload Image
                if (photo != null)
                {
                    bool success = await toyService.UploadToyImage(photo, SelectedToy);
                    if (success)
                    {
                       

						// save the file into local storage
						string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

						using Stream sourceStream = await photo.OpenReadAsync();
						using FileStream localFileStream = File.OpenWrite(localFilePath);

						await sourceStream.CopyToAsync(localFileStream);
                        //SelectedImage = localFilePath;
                        OnPropertyChanged(nameof(SelectedImage));
					}
                   else SelectedImage = backup;
                }



			}
            catch (Exception ex) { }
            }
    }
}
