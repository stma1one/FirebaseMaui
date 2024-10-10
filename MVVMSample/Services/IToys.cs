using MVVMSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMSample.Services;

public interface IToys
{

    //בשלב זה רק לדעת מה הפעולות שניתן לבצע באמצעות השרות
   public  Task<List<Toy>?> GetToys();//החזרת אוסף צעצועים
   public   Task<List<ToyTypes>?> GetToyTypes();//החזרת סוגי צעצועים
    public  Task<List<Toy>> GetToyByType(ToyTypes type);//החזרת אוסף צעצועים לפי סוג
    public  Task<List<Toy>?> GetToysByPriceCondition(double price, bool abovePrice);//החזרת צעצועים מעל או מתחת מחיר
    public  Task<List<Toy>?> GetToysByPriceCondition(Predicate<double> condition);//החזרת צעצועים עפ"י תנאי
    public  Task<Toy> AddToy(Toy toy);//הוספת צעצוע
    public  Task<bool> DeleteToy(Toy toy);//מחיקת צעצוע

    public  Task<User> Login(string username, string password);  //חיבור

    public Task<bool> UploadToyImage(FileResult photo, Toy toy);//העלאת תמונה


}
