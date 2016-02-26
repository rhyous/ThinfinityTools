using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleArgs;
using ThinfinityTools.ProfileManager.Arguments;
using ThinfinityTools.Tests.Fakes;

namespace ThinfinityTools.ProfileManager.Tests
{
    [TestClass]
    public class ArgsHandlerTests
    {
        [TestMethod]
        public void AddProfileTest()
        {
            // Arrange
            var args = new[] { @"AddProfiles=Data\Profiles.xml" };
            var argsHandler = new ArgsHandler();
            var service = new FakeProfileService();
            argsHandler.Repo = new ProfileRepository(service);

            // Act
            ArgsManager.Instance.Start(argsHandler, args);

            // Assert
        }
    }
}
