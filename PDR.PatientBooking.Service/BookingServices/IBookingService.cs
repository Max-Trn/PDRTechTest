using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        public void AddBooking(AddBookingRequest request);
        public GetPatientNextAppointmentRequest GetPatientNextAppointment(long identificationNumber);
    }
}
