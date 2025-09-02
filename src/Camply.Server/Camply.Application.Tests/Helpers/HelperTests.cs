using Camply.Application.Helpers;
using Shouldly;

namespace Camply.Application.Tests.Helpers
{
    public class HelperTests
    {
        [Fact]
        public void GetTimeExisted_Should_Return_Seconds_When_Less_Than_A_Minute()
        {
            var created = DateTime.UtcNow.AddSeconds(-30);

            var result = TimeHelper.GetTimeExisted(created);

            result.ShouldBe("30s");
        }

        [Fact]
        public void GetTimeExisted_Should_Return_Minutes_When_Less_Than_An_Hour()
        {
            var created = DateTime.UtcNow.AddMinutes(-15);

            var result = TimeHelper.GetTimeExisted(created);

            result.ShouldBe("15m");
        }

        [Fact]
        public void GetTimeExisted_Should_Return_Hours_When_Less_Than_A_Day()
        {
            var created = DateTime.UtcNow.AddHours(-5);

            var result = TimeHelper.GetTimeExisted(created);

            result.ShouldBe("5h");
        }

        [Fact]
        public void GetTimeExisted_Should_Return_Days_When_Less_Than_A_Month()
        {
            var created = DateTime.UtcNow.AddDays(-10);

            var result = TimeHelper.GetTimeExisted(created);

            result.ShouldBe("10d");
        }

        [Fact]
        public void GetTimeExisted_Should_Return_Months_When_Less_Than_A_Year()
        {
            var created = DateTime.UtcNow.AddDays(-90);

            var result = TimeHelper.GetTimeExisted(created);

            result.ShouldBe("3mo");
        }

        [Fact]
        public void GetTimeExisted_Should_Return_Years_When_More_Than_A_Year()
        {
            var created = DateTime.UtcNow.AddDays(-800);

            var result = TimeHelper.GetTimeExisted(created);

            result.ShouldBe("2y");
        }
    }
}