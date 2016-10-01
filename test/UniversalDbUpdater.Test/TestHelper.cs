using System;
using Moq;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.Test
{
    public static class TestHelper
    {
        public static Mock<IConsoleFacade> CreateConsoleMock()
        {
            return new Mock<IConsoleFacade>();
        }

        public static Mock<IConsoleFacade> SetupWriteLineToConsole(this Mock<IConsoleFacade> mock)
        {
            mock.Setup(x => x.WriteLine(It.IsAny<string>())).Callback((string s) => Console.WriteLine(s));
            mock.Setup(x => x.WriteLine()).Callback(Console.WriteLine);
            return mock;
        }

        public static Mock<IDateTimeFacade> CreateDateTimeMock(DateTime now)
        {
            var mock = new Mock<IDateTimeFacade>();
            mock.Setup(x => x.Now).Returns(now);

            return mock;
        }
    }
}