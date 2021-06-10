using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.DateTimeProvider;
using PDR.PatientBooking.Service.Validation;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AddBookingRequestValidator(PatientBookingContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public PdrValidationResult ValidateRequest(AddBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (DoctorAlreadyBooked(request, ref result))
                return result;

            if (BookingInThePast(request, ref result))
                return result;

            return result;
        }

        private bool DoctorAlreadyBooked (AddBookingRequest request, ref PdrValidationResult result)
        {
            var timeAlreadyTaken = _context.Order.Any(x =>
               x.DoctorId == request.DoctorId &&
               x.StartTime <= request.EndTime &&
               request.StartTime <= x.EndTime
            );

            if (timeAlreadyTaken)
            {
                result.PassedValidation = false;
                result.Errors.Add("The specified time for the doctor is already booked");
                return true;
            }

            return false;
        }

        private bool BookingInThePast(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (request.StartTime < _dateTimeProvider.DateTimeNow)
            {
                result.PassedValidation = false;
                result.Errors.Add("The specified time of booking is in the past");
                return true;
            }

            return false;
        }
    }
}
