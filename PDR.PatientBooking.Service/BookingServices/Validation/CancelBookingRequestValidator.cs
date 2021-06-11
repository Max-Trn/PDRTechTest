using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.Enums;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class CancelBookingRequestValidator : ICancelBookingRequestValidator
    {
        public PdrValidationResult ValidateRequest(Order order)
        {
            var result = new PdrValidationResult(true);

            if (BookingNotExists(order, ref result))
                return result;

            if (BookingAlreadyCanceled(order, ref result))
                return result;

            return result;
        }

        private bool BookingNotExists(Order order, ref PdrValidationResult result)
        {
            if (order == null)
            {
                result.PassedValidation = false;
                result.Errors.Add("Specified appointment does not exist");
                return true;
            }

            return false;
        }

        private bool BookingAlreadyCanceled(Order order, ref PdrValidationResult result)
        {
            if (order.State == (int)BookingState.Canceled)
            {
                result.PassedValidation = false;
                result.Errors.Add("Specified appointment already canceled");
                return true;
            }

            return false;
        }
    }
}
