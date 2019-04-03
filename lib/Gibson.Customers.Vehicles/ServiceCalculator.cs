using System;
using System.Linq;
using Gibson.Common.Models;

namespace Gibson.Customers.Vehicles
{
    public interface IServiceCalculator
    {
        DateTime CalculateServiceDueDate(CustomerVehicle v);
        int CurrentMileage(CustomerVehicle v);
        int MilesSinceLastService(CustomerVehicle v);
        int DaysRemainingBasedOnMileage(CustomerVehicle v);
        int DaysRemainingBasedOnDate(CustomerVehicle v);
        int ServiceMilesRemaining(CustomerVehicle v);
        int AverageMileagePerDay(CustomerVehicle v);
    }

    public class ServiceCalculator : IServiceCalculator
    {
        public DateTime CalculateServiceDueDate(CustomerVehicle v)
        {
            var daysRemaining = Math.Min(DaysRemainingBasedOnDate(v), DaysRemainingBasedOnMileage(v));
            var dueDate = DateTime.Today.AddDays(daysRemaining);
            return dueDate;
        }

        public int CurrentMileage(CustomerVehicle v)
        {
            var lastMot = v.MotData.MOTResults.OrderByDescending(mot => mot.CompletedDate).FirstOrDefault();
            var daysSinceLastMot = DateTime.Today.Subtract(DateTime.Parse(lastMot.CompletedDate)).Days;

            var avgMileagePerDay = AverageMileagePerDay(v);
            var calcMileageSinceLastMot = daysSinceLastMot * avgMileagePerDay;
            var estCurrentMileage = int.Parse(lastMot.OdometerValue) + calcMileageSinceLastMot;
            return estCurrentMileage;
        }

        public int MilesSinceLastService(CustomerVehicle v)
        {
            var estCurrentMileage = CurrentMileage(v);
            var mileageAtLastService = v.ServiceData.LastServiceMileage;
            return estCurrentMileage - mileageAtLastService;
        }

        public int DaysRemainingBasedOnMileage(CustomerVehicle v)
        {
            // How many miles per day?
            var avgMileagePerDay = AverageMileagePerDay(v);    
            // How many miles left?
            decimal milesLeft = ServiceMilesRemaining(v) - CurrentMileage(v);
            // How many days remaining base on mileage per day
            var daysRemaining = (int)Math.Round(milesLeft / avgMileagePerDay);
            return daysRemaining;
        }

        public int DaysRemainingBasedOnDate(CustomerVehicle v)
        {
            var daysSinceLastService = DateTime.Today.Subtract(v.ServiceData.LastServicedOn.Value).Days;
            var daysRemaining = (v.ServiceData.MaxMonths * 30) - daysSinceLastService;
            return daysRemaining;
        }

        public int ServiceMilesRemaining(CustomerVehicle v)
        {
            var milesSinceLastService = MilesSinceLastService(v);
            return v.ServiceData.MaxMileage - milesSinceLastService;
        }

        public int AverageMileagePerDay(CustomerVehicle v)
        {
            var avgMileagePerDay = v.ServiceData.EstAnualMileage / 365;
            return avgMileagePerDay;
        }
    }
}