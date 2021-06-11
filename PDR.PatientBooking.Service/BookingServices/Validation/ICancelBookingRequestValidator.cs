using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public interface ICancelBookingRequestValidator
    {
        PdrValidationResult ValidateRequest(Order order);
    }
}
