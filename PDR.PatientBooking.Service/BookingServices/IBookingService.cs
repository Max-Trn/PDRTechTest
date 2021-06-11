using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using System;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        public void AddBooking(AddBookingRequest request);
        public void CancelBooking(Guid orderId);
        public GetPatientNextAppointmentResponse GetPatientNextAppointment(long identificationNumber);
    }
}
