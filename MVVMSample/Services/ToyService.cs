using MVVMSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMSample.Services;









//בעתיד הפעולות יפנו לשרת DB
    class ToyService:IToys
    {
        int id;
        List<Toy>? toys;
        List<ToyTypes>? toyTypes;
        public ToyService()
        {
            InitToyTypes();
            InitToys();
            id = 0;
        }

        private  void InitToyTypes()
        {
           toyTypes= new List<ToyTypes>()
        {
            new ToyTypes()
            {
                Id = 1, Name = "פאזל"
            },

            new ToyTypes()
            {
            Id = 2, Name = "משחקי חשיבה"
            },
            new ToyTypes()
            {
            Id = 3, Name = "בובה"
            }
        };
        }

        private void InitToys()
        {
            toys = new List<Toy>()
            {
                new Toy()
                {
                    Id=++id,
                    Image="chuky.jpg",
                    IsSecondHand=false,
                    Name="צאקי",
                    Price=200,
                    Type=toyTypes[2]
                },
                new Toy()
                {
                    Id=++id,
                    Image="puppet.jpeg",
                    IsSecondHand=false,
                    Name="רובי",
                    Price=250,
                    Type=toyTypes[2]
                },
                new Toy()
                {
                    Id=++id,
                    Image="puzzle.jpeg",
                    IsSecondHand=false,
                    Name="גן חיות",
                    Price=250,
                    Type=toyTypes[0]
                },
                new Toy()
                {
                    Id=++id,
                    Image="thinkgame.jpeg",
                    IsSecondHand=true,
                    Name="מבוכים ודרקונים",
                    Price=250,
                    Type=toyTypes[1]
                }

            };
        }

        public async Task< List<Toy>?> GetToys()
        {
            return toys?.ToList();
        }

        public  async Task<List<Toy>?> GetToyByType(ToyTypes type)
        {
            #region By LINQ
            //   return  toys.Where(t=>t.Type.Id==type.Id).ToList();
            #endregion
            List<Toy> result=new();
            foreach (var t in toys)
            {
                if(t.Type.Id==type.Id)
                    result.Add(t);
            }
            return result;
        }


        public async Task<List<Toy>?> GetToysByPriceCondition(double price, bool abovePrice)
        {
            List<Toy> result = new();
            foreach (var t in toys)
            {
                if (abovePrice)
                {
                    if (t.Price > price)
                        result.Add(t);
                }
                else
                    if (t.Price <= price)
                    result.Add(t);
            }
            return result;
        }

        #region Filter By delegate
        public async Task<List<Toy>?> GetToysByPriceCondition(Predicate<double> condition)
        {
            return toys?.Where(x => condition(x.Price)).ToList();
        }
        #endregion

       
    
    
    public async Task<bool> AddToy(Toy toy)
        {
            if (toys != null&&!(toys.Any(t=>t.Name==toy.Name&&t.IsSecondHand==toy.IsSecondHand)))
            {
            if (toy.Image == null)
                toy.Image = "default_image.png";
            toys.Add(toy);
                return true;
            }
            return false;

        }

        public async Task<bool> DeleteToy(Toy toy)
        {
            foreach (var t in toys)
            {
                if(t.Id==toy.Id)
                    return toys.Remove(t);  
            }
            return false;
            #region using LINQ
            // return toys.Remove(toys.Find((x)=>x.Id==toy.Id));
            #endregion
        }

    public async Task<List<ToyTypes>?> GetToyTypes()
    {
      return toyTypes?.ToList();
    }

    public async Task<User> Login(string username, string password)
    {

        return new User() { Name = username, Password = password };
    }

    public async Task<bool> UploadToyImage(FileResult photo, Toy toy)
    {
        throw new NotImplementedException();
    }

	Task<Toy> IToys.AddToy(Toy toy)
	{
		throw new NotImplementedException();
	}
}

