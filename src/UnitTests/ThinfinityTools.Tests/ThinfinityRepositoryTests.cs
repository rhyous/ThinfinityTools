using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools.Tests
{
    [TestClass]
    public class ThinfinityRepositoryTests
    {
        [TestMethod]
        public void RepositoryAllProfilesCallIServiceAllProfiles()
        {
            // Arrange
            var serviceMock = new Mock<IThinfinityService>();
            var setupProfiles = new[] { new WSProfile() };
            serviceMock.Setup(m => m.AllProfiles).Returns(setupProfiles);
            var repo = new ThinfinityRepository(serviceMock.Object);

            // Act
            var profiles = repo.AllProfiles;

            // Assert
            serviceMock.Verify(m => m.AllProfiles);
            Assert.AreEqual(setupProfiles.Length, profiles.Count);
        }
    }
}
