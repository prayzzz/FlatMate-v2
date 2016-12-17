using Microsoft.Extensions.Logging;
using Moq;

namespace prayzzz.Common.Test
{
    public static class TestHelper
    {
        public static Mock<T> Mock<T>(MockBehavior mockBehavior = MockBehavior.Strict) where T : class
        {
            return new Mock<T>(mockBehavior);
        }

        public static Mock<ILoggerFactory> MockLogger<T>()
        {
            var loggerFactory = Mock<ILoggerFactory>(MockBehavior.Loose);
            loggerFactory.Setup(x => x.CreateLogger(typeof(T).FullName)).Returns(new ConsoleLogger());

            return loggerFactory;
        }
    }
}