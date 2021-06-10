using System.Text.RegularExpressions;

namespace PDR.PatientBooking.Service.Validation
{
    public class RegexEmailValidator : IEmailValidator
    {
        public bool IsValid(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+(\.[^@\s]+)+$", RegexOptions.IgnoreCase);
        }
    }
}
