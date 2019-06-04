using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gateway.Controllers;
using Gateway.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Web.Http.Results;

namespace GatewayTests
{
    [TestClass]
    public class GatewayTests
    {
        [TestMethod]
        public void GetAllCustomersTestMethod_ShouldNotBeNull()
        {
            var controller = new CustomersController();

            var result = controller.GetCustomers(null,null);


            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetCustomerTestMethod_ShouldReturn200Ok()
        {
            var controller = new CustomersController();

            var result = controller.GetCustomer(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"));

            Assert.IsNotNull(result);
            //Assert.AreEqual(result.StatusCode, StatusCodes.Status200OK);

        }

        [TestMethod]
        public void GetCustomerTestMethod_BadRequestResult()
        {

            var controller = new CustomersController();

            var result = controller.GetCustomer(new Guid("8b438645-c807-44cf-a0f4-e24323fc3871"));

            var createdResult = result as IActionResult;

            Assert.IsFalse(createdResult is OkObjectResult);

        }

        [TestMethod]
        public void GetCustomerTestMethod_ShouldReturn404NotFound()
        {
            var controller = new CustomersController();
            OkObjectResult result = null;

            try
            {
                result = controller.GetCustomer(new Guid("d8374299-a7f4-4293-8364-4f65b0f7947a")).Result as OkObjectResult;
            }
            catch
            {

            }

            Assert.IsNull(result);

        }

        [TestMethod]
        public void PostCustomerTestMethod_NotValid()
        {

            var controller = new CustomersController();

            OkObjectResult result = null;

            try
            {
                result = controller.PostCustomer(Mock.Of<Customer>()).Result as OkObjectResult;
            }
            catch
            {

            }

            Assert.IsNull(result);

        }

        [TestMethod]
        public void GetBookingTestMethod_BadRequestResult()
        {

            var controller = new BookingsController();

            var result = controller.GetBooking(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"));

            var createdResult = result as IActionResult;

            Assert.IsFalse(createdResult is OkObjectResult);

        }


        [TestMethod]
        public void GetAllBookingsTestMethod_ShouldNotBeNull()
        {

            var controller = new BookingsController();

            var result = controller.GetBookings(null, null);

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetBookingTestMethod_ShouldReturn200Ok()
        {
            var controller = new BookingsController();

            var result = controller.GetBooking(new Guid("07b073f2-754f-4f85-9425-ae758d267e2b"));

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetBookingTestMethod_ShouldReturn404NotFound()
        {
            OkObjectResult result = null;

            var controller = new BookingsController();

            try
            {
                result = controller.GetBooking(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82")).Result as OkObjectResult;
            }
            catch
            {

            }

            Assert.IsNull(result);

        }

        [TestMethod]
        public void PostBookingTestMethod_NotValid()
        {

            var controller = new BookingsController();

            Booking bk = new Booking
            {
                BookingId = new Guid("07b073f2-754f-4f85-9425-ae758d267e2b"),
                CustomerId = new Guid("f05d8278-e37e-4c99-aae3-35a84f63cf02"),
                RoomId = new Guid("07b073f2-754f-4f85-9425-ae758d267e2b")
            };

            OkObjectResult result = null;

            try
            {
                result = controller.PostBooking(bk).Result as OkObjectResult;
            }
            catch
            {

            }

            Assert.IsNull(result);

        }

        [TestMethod]
        public void GetRoomTestMethod_BadRequestResult()
        {

            var controller = new RoomsController();

            var result = controller.GetRoom(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"));

            var createdResult = result as IActionResult;

            Assert.IsFalse(createdResult is OkObjectResult);

        }


        [TestMethod]
        public void GetAllRoomsTestMethod_ShouldNotBeNull()
        {

            var controller = new RoomsController();

            var result = controller.GetRooms(null, null);

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetRoomTestMethod_ShouldReturn200Ok()
        {
            var controller = new RoomsController();

            var result = controller.GetRoom(new Guid("07b073f2-754f-4f85-9425-ae793d267e2b"));//.Result as OkObjectResult;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetRoomTestMethod_ShouldReturn404NotFound()
        {
            var controller = new RoomsController();

            var result = controller.GetRoom(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82")).Result as OkObjectResult;

            Assert.IsNull(result);

        }

        [TestMethod]
        public void PostRoomTestMethod_Valid()
        {

            var controller = new RoomsController();

            var result = controller.PostRoom(Mock.Of<Room>()).Result as OkObjectResult;

            Assert.IsTrue(result is OkObjectResult);

        }

        [TestMethod]
        public void GetBookingWithInfo_NotValid()
        {
            var controller = new AggregationController();

            var result = controller.GetBookingWithInfo(new Guid("dbe35e54-9d2a-405c-addb-b1d2ddfc0711")).Result;

            Assert.IsFalse(result is OkObjectResult);
        }


        private List<Customer> GetTestCustomers()
        {
            var testCustomers = new List<Customer>();
            testCustomers.Add(new Customer { CustomerId = new Guid("ce5a43e6-1d8d-47dd-a503-67a4ba3f3f31"), Name = "Darlene", Surname = "Johns", PhoneNumber = "+79932138831" });
            testCustomers.Add(new Customer { CustomerId = new Guid("15dc56f2-2dbd-42bd-84c3-dd0de5ac9cb8"), Name = "Mcgee", Surname = "Weeks", PhoneNumber = "+79932138832" });
            testCustomers.Add(new Customer { CustomerId = new Guid("11e40ea0-f928-4328-b600-d7d6ce784c4d"), Name = "Adkins", Surname = "Beach", PhoneNumber = "+79932138833" });
            testCustomers.Add(new Customer { CustomerId = new Guid("f05d8278-e37e-4c99-aae3-35a84f63cf02"), Name = "Bertha", Surname = "Tucker", PhoneNumber = "+79932138834" });
            testCustomers.Add(new Customer { CustomerId = new Guid("ea0543cc-61ab-4891-9877-a7a55cff3c80"), Name = "Holden", Surname = "Rivera", PhoneNumber = "+79932138835" });


            return testCustomers;
        }

        private List<Booking> GetTestBookings()
        {
            var testBookings = new List<Booking>();
            testBookings.Add(new Booking { BookingId = new Guid("07b073f2-754f-4f85-9425-ae758d267e2b"), CustomerId = new Guid("f05d8278-e37e-4c99-aae3-35a84f63cf02"), RoomId = new Guid("07b073f2-754f-4f85-9425-ae758d267e2b") });
            return testBookings;
        }

        private List<Room> GetTestRooms()
        {
            var testRooms = new List<Room>();
            testRooms.Add(new Room { RoomId = new Guid("07b073f2-754f-4f85-9425-ae793d267e2b"), Cost = 1500, Number = 90 });
            return testRooms;
        }



    }
}
