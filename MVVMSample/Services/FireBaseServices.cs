﻿using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Firebase.Storage;
using MVVMSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MVVMSample.Services
{
	internal class FirebaseServices : RealTimeService
	{
		#region הזדהות
		FirebaseAuthClient auth;//משמש לצורך ההזדהות
		#endregion
		#region צריכת נתונים
		FirebaseClient client;//משמש אותנו לצורך צריכת נתונים
		#endregion

		# region Storage
		FirebaseStorage storage;
		#endregion

		#region פעולה בונה
		public FirebaseServices()
		{
			var config = new FirebaseAuthConfig()
			{
				ApiKey = "AIzaSyDlGeadVcktnKQCYOyb4r8OY94O7kpCbJo",
				AuthDomain = "toyfactory-9c42f.firebaseapp.com",
				Providers = new FirebaseAuthProvider[]
				   {
					   new EmailProvider()
				   },
				UserRepository = new FileUserRepository("appuser")//persist data into %AppData%\appuser
			};
			auth = new FirebaseAuthClient(config);
			#region הגדרת הקליינט
			client = new FirebaseClient(@"https://toyfactory-9c42f-default-rtdb.europe-west1.firebasedatabase.app/",	
  new FirebaseOptions
  {
	  AuthTokenAsyncFactory = () => Task.FromResult(auth.User.Credential.IdToken)
  });
			#endregion

			#region הגדרת האחסון
			storage = new FirebaseStorage("toyfactory-9c42f.appspot.com",
							new FirebaseStorageOptions()
							{
								AuthTokenAsyncFactory = () => Task.FromResult(auth.User.Credential.IdToken),
								ThrowOnCancel = true
							});
			#endregion
		}
		#endregion

		#region התחברות
		public async Task<Models.User> Login(string username, string password)
		{
			try
			{
				var authUser = await auth.SignInWithEmailAndPasswordAsync(username, password);
			
				if(authUser!=null)
					return new Models.User() { Email = authUser.User.Info.Email, Name = authUser.User.Info.FirstName + " " + authUser.User.Info.LastName };
			}
			catch (Exception ex) { return null; }
			return null;
				
		}
		#endregion


		#region הוספת צעצוע
		public async Task<Toy> AddToy(Toy toy)
		{
			try
			{
				
				var result = await client.Child("Toys").PostAsync(new { Price = toy.Price, Name=toy.Name, ToyTypeKey=toy.ToyTypeKey, Image=toy.Image, IsSecondHand=toy.IsSecondHand }, false);
				toy.FirebaseKey = result.Key;
				//await client.Child("Toys").Child($"{toy.FirebaseKey}").PutAsync<Toy>(toy);
				return toy;
			}
			catch (Exception ex) { await Shell.Current.DisplayAlert("Err", $"{ex.Message}", "OK"); }
				return null;

		}
		#endregion

		#region מחיקת צעצוע
		public async Task<bool> DeleteToy(Toy toy)
		{
			try
			{
				await client.Child("Toys").Child($"{toy.FirebaseKey}").DeleteAsync();
				return true;
			}
			catch (Exception ex) {Shell.Current.DisplayAlert("Err", $"{ex.Message}", "OK"); }
			return false;
		}

		#endregion

		#region אחזור סוגי צעצועים
		public async Task<List<ToyTypes>?> GetToyTypes()
		{
			try
			{
				var toyTypes = await client.Child("ToyTypes").OnceAsync<ToyTypes>();
				//var types = JsonSerializer.Deserialize<List<ToyTypes>>(toyTypes, new JsonSerializerOptions() { PropertyNameCaseInsensitive=true });

				//List<ToyTypes> types = new List<ToyTypes>();
				//foreach (var type in toyTypes)
				//{
				//	types.Add(type.Object);
				//}
				return toyTypes?.Select(item => new ToyTypes() { Id = int.Parse(item.Key), Name = item.Object.Name }).ToList();

			}
			catch (Exception ex) { return null; }
			return null;
		}
		#endregion

		#region פעולות נוספות...
		public Task<List<Toy>> GetToyByType(ToyTypes type)
		{
			throw new NotImplementedException();
		}

		public Task<List<Toy>?> GetToys()
		{
			throw new NotImplementedException();
		}

		public Task<List<Toy>?> GetToysByPriceCondition(double price, bool abovePrice)
		{
			throw new NotImplementedException();
		}

		public Task<List<Toy>?> GetToysByPriceCondition(Predicate<double> condition)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region קבלת עדכונים אוטומטית
		public IObservable<FirebaseEvent<Toy>> RegisterToys()
		{
			return client.Child("Toys").AsObservable<Toy>();
		}
		#endregion

		#region העלאת תמונה לאחסון
		public async Task<bool> UploadToyImage(FileResult photo, Toy toy)
		{
			try
			{
				    Stream stream = await photo.OpenReadAsync();
				
					string url = await storage.Child("Profiles").Child($"{toy.FirebaseKey}").Child($"{photo.FileName}").PutAsync(stream);
				//toy.Image = url;
				await client.Child($"Toys").Child($"{
					toy.FirebaseKey}").Child("Image").PutAsync<string>(url);
					toy.Image = url;
					return true;
				
			}
			catch (Exception ex) { return false; }	
		}
		#endregion
	}
}
