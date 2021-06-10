using System;
using System.Collections.Generic;
using System.Text;

namespace PDR.PatientBooking.Service.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        public DateTime DateTimeNow { get; }
    }
}
