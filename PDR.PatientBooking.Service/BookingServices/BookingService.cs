﻿using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _validator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator validatior)
        {
            _context = context;
            _validator = validatior;
        }

        public void AddBooking(AddBookingRequest request)
        {
            var validationResult = _validator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }

            var bookingId = new Guid();
            var bookingStartTime = request.StartTime;
            var bookingEndTime = request.EndTime;
            var bookingPatientId = request.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId);
            var bookingDoctorId = request.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId);
            var bookingSurgeryType = bookingPatient.Clinic.SurgeryType;

            _context.Order.Add(new Order
            {
                Id = bookingId,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime,
                PatientId = bookingPatientId,
                DoctorId = bookingDoctorId,
                Patient = bookingPatient,
                Doctor = bookingDoctor,
                SurgeryType = (int)bookingSurgeryType
            });

            _context.SaveChanges();
        }

        public GetPatientNextAppointmentResponse GetPatientNextAppointment(long identificationNumber)
        {
            var bookings = _context.Order.OrderBy(x => x.StartTime).ToList();
            var result = bookings.FirstOrDefault(x => x.PatientId == identificationNumber && x.StartTime > DateTime.Now);
            
            if (result is null) throw new ArgumentException();

            return new GetPatientNextAppointmentResponse()
            {
                Id = result.Id,
                DoctorId = result.DoctorId,
                StartTime = result.StartTime,
                EndTime = result.EndTime
            };
        }
    }
}