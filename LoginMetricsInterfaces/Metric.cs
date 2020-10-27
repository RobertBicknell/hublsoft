using System;
using System.Collections.Generic;
using System.Text;

namespace LoginMetricsInterfaces
{
    public struct Metric
    {
        public string Subject;
        public float Value;

        public Metric(string subject, float value) {
            Subject = subject;
            Value = value;
        }
    }
}
