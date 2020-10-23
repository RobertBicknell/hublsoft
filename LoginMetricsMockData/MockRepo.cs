using System;
using System.Collections.Generic;
using LoginMetricsInterfaces;

namespace LoginMetricsMockData
{
    public class MockRepo : ILoginMetricsRepo
    {
        List<LoginRecord> _data;

        public MockRepo() {
            _data = new List<LoginRecord>();
            var users = new Dictionary<string, int>();
            users["Alice"] = 10;
            users["Bob"] = 3;
            users["Charlie"] = 7;
            foreach (var user in users) {
                for (var i = 1; i <= user.Value; i++)
                {
                    var loginTime = new DateTime(2020, 10, i, i % 24, i % 60, i % 60);
                    var loginRecord = new LoginRecord(user.Key, loginTime);
                    _data.Add(loginRecord);
                }
            }
            var loginTime1 = new DateTime(2020, 10, 1);
            var loginRecord1 = new LoginRecord("Alice", loginTime1);
            _data.Add(loginRecord1);

            var loginTime2 = new DateTime(2020, 10, 1);
            var loginRecord2 = new LoginRecord("Alice", loginTime2);
            _data.Add(loginRecord2);

            var loginTime3 = new DateTime(2020, 10, 1);
            var loginRecord3 = new LoginRecord("Alice", loginTime3);
            _data.Add(loginRecord3);


        }
        public List<LoginRecord> GetUserLoginsInPeriod(DateTime start, DateTime end, List<string> users)
        {
            var result = new List<LoginRecord>();
            Func<string, bool> checker = (user) => { if (users != null) { return users.Contains(user); } else { return true; } };
            foreach (var loginRecord in _data) {
                if (loginRecord.LoginTime >= start && loginRecord.LoginTime < end && checker(loginRecord.User)) { 
                    result.Add(loginRecord);
                }
            }
            return result;
        }
    }
}
