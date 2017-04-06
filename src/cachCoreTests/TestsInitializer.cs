using NUnit.Framework;

namespace cachCoreTests
{
    [SetUpFixture]
    public class TestsInitializer
    {
        [OneTimeSetUp]
        public void InitializeLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
