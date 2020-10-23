using System;
using System.Collections.Generic;
using LoginMetricsInterfaces;

namespace LoginMetricsInterfaces
{
    public interface ILoginMetricsRepo
    {
        List<LoginRecord> GetUserLoginsInPeriod(DateTime start, DateTime end, List<string> users);
    }
}
