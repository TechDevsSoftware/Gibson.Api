using System;
using System.Collections.Generic;
using Gibson.Common.Models;
using Xunit;

namespace Gibson.Customers.Vehicles.Tests
{
    public class ServiceCalculatorTests
    {
        private IServiceCalculator GetSut() => new ServiceCalculator();
        
        [Fact]
        public void ShouldProduceCorrectResult()
        {
            // Arrange
            var vehicle = new CustomerVehicle
            {
                ServiceData = new ServiceData
                {
                    EstAnualMileage = 10000,
                    MaxMonths = 18,
                    MaxMileage = 12000,
                    LastServicedOn = DateTime.Parse("2018-01-01"), // Due 2019-06-01
                    LastServiceMileage = 100000                    
                },
                MotData = new MotData
                {
                    MOTExpiryDate = DateTime.Parse("2019-04-01"),
                    MOTResults = new List<MotResult>
                    {
                        new MotResult
                        {
                            CompletedDate = "2018-04-01",
                            OdometerUnit = "",
                            OdometerValue = "105000"
                        }
                    }
                }
            };
            var sut = GetSut();
            // Act
            var result = sut.CalculateServiceDueDate(vehicle);
            // Assert
            
        }
    }
}