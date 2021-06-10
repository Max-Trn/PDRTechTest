using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using PDR.PatientBooking.Service.DateTimeProvider;
using System;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    public class AddBookingRequestValidatorTests
    {
        private IFixture _fixture;
        private MockRepository _mockRepository;

        private PatientBookingContext _context;

        private Mock<IDateTimeProvider> _dateTimeProvider;

        private IAddBookingRequestValidator _addBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();
            _mockRepository = new MockRepository(MockBehavior.Strict);

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _dateTimeProvider = _mockRepository.Create<IDateTimeProvider>();

            SetupMockDefaults();

            // Sut instantiation
            _addBookingRequestValidator = new AddBookingRequestValidator(
                _context,
                _dateTimeProvider.Object
            );
        }

        private void SetupMockDefaults()
        {
            _dateTimeProvider.Setup(x => x.DateTimeNow).Returns(DateTime.Now);
        }

        [Test]
        public void ValidateRequest_TimeIsPast_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = _dateTimeProvider.Object.DateTimeNow.AddHours(-2);
            request.EndTime = _dateTimeProvider.Object.DateTimeNow.AddHours(-1);
            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("The specified time of booking is in the past");
        }

        [TestCase(3,4)] // new time period inside
        [TestCase(0,7)] // outside
        [TestCase(0,3)] // before
        [TestCase(0,2)] // before and touching
        [TestCase(5,7)] // after
        [TestCase(6,7)] // after and touching
        [TestCase(2,6)] // same period
        public void ValidateRequest_DoctorAlreadyBooked_ReturnsFailedValidationResult(int startHourOffset, int endHourOffset)
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = _dateTimeProvider.Object.DateTimeNow.AddHours(startHourOffset);
            request.EndTime = _dateTimeProvider.Object.DateTimeNow.AddHours(endHourOffset);
            request.DoctorId = 1;

            SeedData();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("The specified time for the doctor is already booked");
        }

        [Test]
        public void ValidateRequest_AllCheckPassed_ReturnsPassedValidationResult()
        {
            //arrange
            var request = GetValidRequest();
            SeedData();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeTrue();
        }

        private void SeedData()
        {
            _context.Order.Add(new Order()
            {
                PatientId = 1,
                DoctorId = 1,
                StartTime = _dateTimeProvider.Object.DateTimeNow.AddHours(2),
                EndTime = _dateTimeProvider.Object.DateTimeNow.AddHours(6),
            });
            _context.Order.Add(new Order()
            {
                PatientId = 2,
                DoctorId = 2,
                StartTime = _dateTimeProvider.Object.DateTimeNow.AddHours(0),
                EndTime = _dateTimeProvider.Object.DateTimeNow.AddHours(2),
            });

            _context.SaveChanges();
        }
        private AddBookingRequest GetValidRequest()
        {
            var request = _fixture.Build<AddBookingRequest>()
                .With(x => x.StartTime, _dateTimeProvider.Object.DateTimeNow)
                .With(x => x.EndTime, _dateTimeProvider.Object.DateTimeNow.AddHours(1))
                .Create();
            return request;
        }
    }
}
