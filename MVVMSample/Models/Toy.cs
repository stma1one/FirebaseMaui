using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MVVMSample.Models;

/// <summary>
/// מחלקה המייצגת צעצוע
/// </summary>
 public   class Toy
    {

	#region Firebase keys
	//FirebaseKey
	public string? FirebaseKey
    {
        get; set;
    }
    //ToyTypes Firebase Key
    public string? ToyTypeKey
    {
        get; set;
    }
	#endregion
	public int Id { get; set; }//קוד זיהוי
        public string? Name { get; set; }    //שם צעצוע
        public double Price {  get; set; }  //מחיר
        public string? Image { get; set; }//קישור לתמונה
    [JsonIgnore]
        public ToyTypes? Type { get; set; }//סוג צעצוע
        public bool IsSecondHand { get; set; }//האם צעצוע יד שניה


}

