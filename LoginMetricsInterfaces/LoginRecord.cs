using System;
using System.Collections.Generic;
using System.Text;

namespace LoginMetricsInterfaces
{
    public class LoginRecord
    {
        public DateTime LoginTime;
        public string User;

        public LoginRecord(string user, DateTime loginTime) {
            LoginTime = loginTime;
            User = user;
        }
    }
}
