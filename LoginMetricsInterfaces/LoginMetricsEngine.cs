using System;
using System.Collections.Generic;
using System.Text;

namespace LoginMetricsInterfaces
{
    public interface ILoginMetricsEngine
    {
        List<Metric> TopNLoginsInPeriod(DateTime start, DateTime end, int N);
        List<Metric> AverageLoginsByPeriod(DateTime start, DateTime end, Period period);
        List<Metric> AverageUserLoginsByPeriod(DateTime start, DateTime end, Period period, string user);

    }
}
