using System;
using System.Collections.Generic;
using Xunit;
using LoginMetricsMockData;
using LoginMetrics;
using LoginMetricsInterfaces;

namespace LoginMetricsTest
{
    public class UnitTests
    {
        ILoginMetricsRepo _testRepo;
        public UnitTests() {
            _testRepo = new MockRepo(); //is read only, so stateless with respect to tests
        }

        void CheckAnswerEqualsResult(List<Metric> answer, List<Metric> result) {
            Assert.Equal(answer.Count, result.Count);
            for (var i = 0; i < answer.Count; i++)
            {
                Assert.True(answer[i].Subject == result[i].Subject);
                Assert.True(answer[i].Value == result[i].Value);
            }
        }

        [Fact]
        public void Test_Top2LoginsInOctober()
        {
            //Define answer
            var top = new Metric("Alice", 13);
            var next = new Metric("Charlie", 7);
            var answer = new List<Metric>();
            answer.Add(top);
            answer.Add(next);

            //Arrange
            var metricsEngine = new LoginMetricsEngine(_testRepo);

            //Act
            var result = metricsEngine.TopNLoginsInPeriod(new DateTime(2020, 10, 1), new DateTime(2020, 10, 31), 2);

            //Assert
            CheckAnswerEqualsResult(answer, result);
        }

        [Fact]
        public void Test_AverageWeeklyLoginsFirstFortnightInOctober()
        {
            //Define answer
            var answer = new List<Metric>();
            var wk1 = new Metric("Wk40", 14);
            var wk2 = new Metric("Wk41", 9);
            var wk3 = new Metric("Wk42", 0);
            answer.Add(wk1);
            answer.Add(wk2);
            answer.Add(wk3);

            //Arrange
            var metricsEngine = new LoginMetricsEngine(_testRepo);

            //Act
            var result = metricsEngine.AverageLoginsByPeriod(new DateTime(2020, 10, 1), new DateTime(2020, 10, 15), Period.Week);

            //Assert
            CheckAnswerEqualsResult(answer, result);
        }

        [Fact]
        public void Test_AverageAliceDailyLoginsInOctoberFirstFortnight()
        {
            //Define answer
            var answer = new List<Metric>();
            var day1 = new Metric("Monday", 1);
            var day2 = new Metric("Tuesday", 1);
            var day3 = new Metric("Wednesday", 1);
            var day4 = new Metric("Thursday", 2.5f);
            var day5 = new Metric("Friday", 1);
            var day6 = new Metric("Saturday", 1);
            var day7 = new Metric("Sunday", 1);

            answer.Add(day1);
            answer.Add(day2);
            answer.Add(day3);
            answer.Add(day4);
            answer.Add(day5);
            answer.Add(day6);
            answer.Add(day7);

            //Arrange
            var metricsEngine = new LoginMetricsEngine(_testRepo);

            //Act
            var result = metricsEngine.AverageUserLoginsByPeriod(new DateTime(2020, 10, 1), new DateTime(2020, 10, 15), Period.Day, "Alice");

            //Assert
            CheckAnswerEqualsResult(answer, result);
        }
    }
}
