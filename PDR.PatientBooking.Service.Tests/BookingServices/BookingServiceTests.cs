using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.DateTimeProvider;
using System;

namespace PDR.PatientBooking.Service.Tests.BookingServices
{
    [TestFixture]
    public class BookingServiceTests
    {
        private MockRepository _mockRepository;
        private IFixture _fixture;

        private PatientBookingContext _context;
        private Mock<IDateTimeProvider> _dateTimeProvider;

        private BookingService _bookingService;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _dateTimeProvider = _mockRepository.Create<IDateTimeProvider>();

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _bookingService = new BookingService(
                _context
            );
        }

        private void SetupMockDefaults()
        {
        }

        [Test]
        public void AddBooking_AddsBookingToContextWithGeneratedId()
        {
            //arrange
            var request = _fixture.Create<AddBookingRequest>();
            var doctor = _fixture.Build<Doctor>().Without(x => x.Orders).Create();
            var patient = _fixture.Build<Patient>().Without(x => x.Orders).Create();

            _context.Doctor.Add(doctor);
            _context.Patient.Add(patient);
            _context.SaveChanges();

            request.DoctorId = doctor.Id;
            request.PatientId = patient.Id;

            var expected = new Order
            {
                Doctor = doctor,
                Patient = patient,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                SurgeryType = (int)patient.Clinic.SurgeryType
            };

            //act
            _bookingService.AddBooking(request);

            //assert
            _context.Order.Should().ContainEquivalentOf(expected, options => options.Excluding(order => order.Id));
        }
    }
}
