using System;
using System.Globalization;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
	public class DriversRepository
	{
		public Driver GetDriverById(int driverId)
		{
			if (driverId == 15)
			{
                return new Driver(
					driverId,
					new PersonName("Drive", "Driverson"),
					new Car("A123BT 66", "Lada sedan", "Baklazhan")
					);
			}
			else
				throw new Exception("Unknown driver id " + driverId);
		}
	}

	public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private readonly DriversRepository driversRepo;
		private readonly Func<DateTime> currentTime;
		private int idCounter;

		public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
		{
			this.driversRepo = driversRepo;
			this.currentTime = currentTime;
		}

		public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building)
		{
			var client = new PersonName(firstName, lastName);
			var start = new Address(street, building);
			return new TaxiOrder(idCounter++, client, start, null, currentTime());
		}

		public void UpdateDestination(TaxiOrder order, string street, string building)
		{
			order.UpdateDestination(new Address(street, building));
		}

		public void AssignDriver(TaxiOrder order, int driverId)
		{
			order.AssignDriver(driversRepo.GetDriverById(driverId), currentTime());
		}

		public void UnassignDriver(TaxiOrder order)
		{
			order.UnassignDriver();
		}

		public string GetDriverFullInfo(TaxiOrder order)
		{
			return order.GetDriverFullInfo();
		}

		public string GetShortOrderInfo(TaxiOrder order)
		{
			return order.GetShortOrderInfo();
		}

		public void Cancel(TaxiOrder order)
		{
			order.Cancel(currentTime());
		}

		public void StartRide(TaxiOrder order)
		{
			order.StartRide(currentTime());
		}

		public void FinishRide(TaxiOrder order)
		{
			order.FinishRide(currentTime());
		}
	}

	public class TaxiOrder : Entity<int>
	{
		public PersonName ClientName { get; private set; }
		public Driver Driver { get; private set; }
		public Address Start { get; private set; }
		public Address Destination { get; private set; }
		public TaxiOrderStatus Status { get; private set; }
		public OrderTimeline Timeline { get; private set; }

		public TaxiOrder(int id, PersonName clientName, Address start, Address destination, DateTime currentTime) 
			: base(id)
        {
			ClientName = clientName;
			Start = start;
			Destination = destination;
			Status = TaxiOrderStatus.WaitingForDriver;
			Timeline = new OrderTimeline() { CreationTime = currentTime };
        }

		public void UpdateDestination(Address address)
        {
			Destination = address;
        }

		public void AssignDriver(Driver driver, DateTime time)
        {
			if (Status != TaxiOrderStatus.WaitingForDriver) 
				throw new InvalidOperationException(Status.ToString());
			Driver = driver;
			Status = TaxiOrderStatus.WaitingCarArrival;
			Timeline.DriverAssignmentTime = time;
		}

		public void UnassignDriver()
        {
			if (Status != TaxiOrderStatus.WaitingCarArrival)
				throw new InvalidOperationException(Status.ToString());
			Driver = null;
			Status = TaxiOrderStatus.WaitingForDriver;
        }

		public string GetDriverFullInfo()
		{
			if (Status == TaxiOrderStatus.WaitingForDriver) 
				throw new InvalidOperationException(Status.ToString());
			return string.Join(" ",
				$"Id: {Driver.Id}",
				$"DriverName: {Driver.Name.Format()}",
				$"Color: {Driver.Car.Color}",
				$"CarModel: {Driver.Car.Model}",
				$"PlateNumber: {Driver.Car.PlateNumber}"
				);
		}

		public string GetShortOrderInfo()
		{
			return string.Join(" ",
				$"OrderId: {Id}",
				$"Status: {Status}",
				$"Client: {ClientName.Format()}",
				$"Driver: {Driver?.Name.Format()}",
				$"From: {Start.Format()}",
				$"To: {Destination.Format()}",
				$"LastProgressTime: {GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}"
				);
		}

		private DateTime GetLastProgressTime()
		{
			if (Status == TaxiOrderStatus.WaitingForDriver) return Timeline.CreationTime;
			if (Status == TaxiOrderStatus.WaitingCarArrival) return Timeline.DriverAssignmentTime;
			if (Status == TaxiOrderStatus.InProgress) return Timeline.StartRideTime;
			if (Status == TaxiOrderStatus.Finished) return Timeline.FinishRideTime;
			if (Status == TaxiOrderStatus.Canceled) return Timeline.CancelTime;
			throw new NotSupportedException(Status.ToString());
		}

		public void Cancel(DateTime time)
		{
			if (Status != TaxiOrderStatus.WaitingForDriver &&
				Status != TaxiOrderStatus.WaitingCarArrival)
				throw new InvalidOperationException(Status.ToString());
			Status = TaxiOrderStatus.Canceled;
			Timeline.CancelTime = time;
		}

		public void StartRide(DateTime time)
		{
			if (Status != TaxiOrderStatus.WaitingCarArrival)
				throw new InvalidOperationException(Status.ToString());
			Status = TaxiOrderStatus.InProgress;
			Timeline.StartRideTime = time;
		}

		public void FinishRide(DateTime time)
		{
			if (Status != TaxiOrderStatus.InProgress)
                throw new InvalidOperationException(Status.ToString());
			Status = TaxiOrderStatus.Finished;
			Timeline.FinishRideTime = time;
		}
	}

	public class Driver : Entity<int>
    {
		public PersonName Name { get; }
		public Car Car { get; }
		public Driver(int id, PersonName name, Car car) : base(id)
        {
			Name = name;
			Car = car;
        }
    }

	public class Car : ValueType<Car>
    {
		public string PlateNumber { get; }
		public string Model { get; }
		public string Color { get; }
		public Car(string plateNumber, string model, string color)
        {
			PlateNumber = plateNumber;
			Model = model;
			Color = color;
        }
    }

	public class OrderTimeline : ValueType<OrderTimeline>
    {
		public DateTime CreationTime { get; set; }
		public DateTime DriverAssignmentTime { get; set; }
		public DateTime CancelTime { get; set; }
		public DateTime StartRideTime { get; set; }
		public DateTime FinishRideTime { get; set; }
	}

	public static class PersonNameExtension
	{
		public static string Format(this PersonName personName)
		{
			return personName != null 
				? $"{personName.FirstName} {personName.LastName}"
				: "";
		}
	}

	public static class AddressExtension
	{
		public static string Format(this Address address)
		{
			return address != null
				? $"{address.Street} {address.Building}"
				: "";
		}
	}
}
