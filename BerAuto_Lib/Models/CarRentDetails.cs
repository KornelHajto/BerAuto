using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerAuto.Models
{
    public class CarRentDetails
    {
		public int Index { get; set; }
		public Guid RenterID { get; set; }
		public Guid CarID { get; set; }
		public User? Renter { get; set; }
		public Car? RentedCar { get; set; }
		public ERentStatus Status { get; set; }
		public DateTime ApplicationTime { get; set; }
		public int Owed { get; set; } // fizetendő
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public CarRentDetails(CarRent carRent, Rent rent, int i) {
			RentedCar = carRent.Car;
			Renter = rent.Renter;
			RenterID = rent.RenterID;
			CarID = carRent.CarID;
			Status = rent.Status;
			ApplicationTime = rent.ApplicationTime;
			Owed = rent.Owed;
			StartDate = carRent.StartDate;
			EndDate = carRent.EndDate;
			Index = i;
		}
		public override string ToString()
		{
			return $"RenterID: {RenterID}, CarID: {CarID}, Status: {Status}, ApplicationTime: {ApplicationTime}, Owed: {Owed}, StartDate: {StartDate}, EndDate: {EndDate}";
		}
	}
}
