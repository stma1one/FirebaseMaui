using Firebase.Database.Streaming;
using MVVMSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMSample.Services
{
	public interface RealTimeService:IToys
	{
		public IObservable<FirebaseEvent<Toy>> RegisterToys();
	}
}
