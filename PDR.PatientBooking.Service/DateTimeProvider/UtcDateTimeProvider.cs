using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.DateTimeProvider
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime DateTimeNow => DateTime.UtcNow;
    }
}
