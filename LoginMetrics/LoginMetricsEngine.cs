using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LoginMetricsInterfaces;

namespace LoginMetrics
{
    public class LoginMetricsEngine : ILoginMetricsEngine
    {
        ILoginMetricsRepo _repo = null;
        public LoginMetricsEngine(ILoginMetricsRepo repo) {
            _repo = repo;
        }

        public List<Metric> TopNLoginsInPeriod(DateTime start, DateTime end, int N)
        {
            var result = new List<Metric>();
            var userLoginMap = new Dictionary<string, int>();
            var data = _repo.GetUserLoginsInPeriod(start, end, null);
            foreach (var loginrecord in data)
            {
                var user = loginrecord.User;
                if (!userLoginMap.ContainsKey(user)){
                    userLoginMap[user] = 0;
                }
                userLoginMap[user] += 1;
            }
            var sortedUserLoginList = userLoginMap.ToList();
            sortedUserLoginList.Sort((e1, e2) => e2.Value.CompareTo(e1.Value)); //sort descending
            for (var n=0; n < N ; n++) {
                result.Add(new Metric(sortedUserLoginList[n].Key, sortedUserLoginList[n].Value));
            }
            return result; 
        }

        DateTime getPeriodStartDateFromDate(DateTime date, Period period)
        {
            int offSet;
            switch (period)
            {
                case Period.Day:
                    return date;
                case Period.Week:
                    switch (date.DayOfWeek.ToString())
                    {
                        case "Monday":
                            offSet= 0;
                            break;
                        case "Tuesday":
                            offSet = 1;
                            break;
                        case "Wednesday":
                            offSet = 2;
                            break;
                        case "Thursday":
                            offSet = 3;
                            break;
                        case "Friday":
                            offSet = 4;
                            break;
                        case "Saturday":
                            offSet = 5;
                            break;
                        case "Sunday":
                            offSet = 6;
                            break;
                        default:
                            throw new Exception("Cannot determine period id from date: " + date.ToString());
                    }
                    return date.AddDays(offSet * -1);
                case Period.Month:
                    return date.AddDays(-1 * date.Day);
                default:
                    throw new Exception("Cannot determine period id from date: " + date.ToString());
            }
        }

        int getPeriodIdFromDate(DateTime date, Period period) {
            switch (period) {
                case Period.Day:
                    switch (date.DayOfWeek.ToString()) {
                        case "Monday":
                            return 0;
                        case "Tuesday":
                            return 1;
                        case "Wednesday":
                            return 2;
                        case "Thursday":
                            return 3;
                        case "Friday":
                            return 4;
                        case "Saturday":
                            return 5;
                        case "Sunday":
                            return 6;
                        default:
                            throw new Exception("Cannot determine period id from date: " + date.ToString());
                    }
                case Period.Week:
                    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo; 
                    Calendar cal = dfi.Calendar;
                    return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                case Period.Month:
                    return date.Month;
                default:
                    throw new Exception("Cannot determine period id from date: " + date.ToString());
            }
        }

        string getPeriodIdDescriptionFromPeriodId(int periodId, Period period)
        {
            switch (period)
            {
                case Period.Day:
                    switch (periodId)
                    {
                        case 0:
                            return "Monday";
                        case 1:
                            return "Tuesday";
                        case 2:
                            return "Wednesday";
                        case 3:
                            return "Thursday";
                        case 4:
                            return "Friday";
                        case 5:
                            return "Saturday";
                        case 6:
                            return "Sunday";
                        default:
                            throw new Exception("Cannot determine period descriptor from periodId " + periodId);
                    }
                case Period.Week:
                    return "Wk" + periodId;
                case Period.Month:
                    switch (periodId) {
                        case 1:
                            return "January";
                        case 2:
                            return "February";
                        case 3:
                            return "March";
                        case 4:
                            return "April";
                        case 5:
                            return "May";
                        case 6:
                            return "June";
                        case 7:
                            return "July";
                        case 8:
                            return "August";
                        case 9:
                            return "September";
                        case 10:
                            return "October";
                        case 11:
                            return "November";
                        case 12:
                            return "December";
                        default:
                            throw new Exception("Cannot determine month for periodiI " + periodId);
                    }
                default:
                    throw new Exception("Cannot determine period descriptior from periodId " + periodId);

            }
        }

        List<Metric> averageLoginsByPeriod(DateTime start, DateTime end, Period period, List<LoginRecord> data) {
            var result = new List<Metric>();
            data.Sort((d1, d2) => d1.LoginTime.CompareTo(d2.LoginTime));
            var dateToLoginCountMap = new Dictionary<DateTime, int>();
            var periodToPeriodStartDatesMap = new Dictionary<int, List<DateTime>>();
            foreach (var loginrecord in data)
            {
                var date = loginrecord.LoginTime.Date;
                if (!dateToLoginCountMap.ContainsKey(date)) { dateToLoginCountMap[date] = 0; }
                dateToLoginCountMap[date] += 1;
                var periodId = getPeriodIdFromDate(date, period);
                if (!periodToPeriodStartDatesMap.ContainsKey(periodId)) { periodToPeriodStartDatesMap[periodId] = new List<DateTime>(); }
                var periodStartDate = getPeriodStartDateFromDate(date, period);
                if (!periodToPeriodStartDatesMap[periodId].Contains(periodStartDate)) { periodToPeriodStartDatesMap[periodId].Add(periodStartDate); }
            }
            var periodToLoginsMap = new Dictionary<int, int>();
            foreach (var date in dateToLoginCountMap.Keys)
            {
                var periodId = getPeriodIdFromDate(date, period);
                if (!periodToLoginsMap.ContainsKey(periodId)) { periodToLoginsMap[periodId] = 0; }
                periodToLoginsMap[periodId] += dateToLoginCountMap[date];
            }
            var periodToAvgMap = new Dictionary<int, float>();
            foreach (var mapPeriod in periodToLoginsMap.Keys)
            {
                var avg = ((float)Convert.ToDouble(periodToLoginsMap[mapPeriod])) / periodToPeriodStartDatesMap[mapPeriod].Count;
                periodToAvgMap[mapPeriod] = avg;
            }
            var allPeriodIdsInRange = new List<int>();
            var range = (end - start).TotalDays - 1; 
            for (var d=0; d< range; d++) {
                allPeriodIdsInRange.Add(getPeriodIdFromDate(start.AddDays(d), period)); 
            }
            foreach (var p in allPeriodIdsInRange) {
                if (!periodToAvgMap.ContainsKey(p)){
                    periodToAvgMap[p] = 0;
                }
            }
            var sortedAverges = periodToAvgMap.ToList();
            sortedAverges.Sort((d1, d2) => d1.Key.CompareTo(d2.Key)); //todo: possibly redundant...
            foreach (var periodAvg in sortedAverges) {
                result.Add(new Metric(getPeriodIdDescriptionFromPeriodId(periodAvg.Key, period), periodAvg.Value));
            }
            return result;
        }

        public List<Metric> AverageLoginsByPeriod(DateTime start, DateTime end, Period period)
        {
            var data = _repo.GetUserLoginsInPeriod(start, end, null);
            return averageLoginsByPeriod(start, end, period, data);
        }

        public List<Metric> AverageUserLoginsByPeriod(DateTime start, DateTime end, Period period, string user)
        {
            var users = new List<string>();
            users.Add(user);
            var data = _repo.GetUserLoginsInPeriod(start, end, users);
            return averageLoginsByPeriod(start, end, period, data);
        }
    }
}
