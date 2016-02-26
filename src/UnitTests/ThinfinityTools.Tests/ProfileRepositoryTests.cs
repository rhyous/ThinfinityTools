using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThinfinityTools.Tests.Fakes;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools.Tests
{
    [TestClass]
    public class ProfileRepositoryTests
    {
        [TestMethod]
        public void RepositoryAllProfilesCallIServiceAllProfilesTest()
        {
            // Arrange
            var serviceMock = new Mock<IProfileService>();
            var setupProfiles = new[] { new WSProfile() };
            serviceMock.Setup(m => m.AllProfiles).Returns(setupProfiles);
            var repo = new ProfileRepository(serviceMock.Object);

            // Act
            var profiles = repo.AllProfiles;

            // Assert
            serviceMock.Verify(m => m.AllProfiles);
            Assert.AreEqual(setupProfiles.Length, profiles.Count);
        }

        [TestMethod]
        public void RepositoryAddProfilesTest()
        {
            // Arrange
            var fakeProfileService = new FakeProfileService();
            var toBeAddedProfiles = new[] { new WSProfile() { Name = "Prof A" }, new WSProfile() { Name = "Prof B" } };
            var repo = new ProfileRepository(fakeProfileService);

            // Act
            var profiles = repo.AddProfiles(toBeAddedProfiles);

            // Assert
            Assert.AreEqual(toBeAddedProfiles.Length, profiles.Count);
            Assert.AreEqual(toBeAddedProfiles.Length, fakeProfileService.AllProfiles.Length);
        }

        [TestMethod]
        public void RepositoryDeleteProfilesByNameTest()
        {
            // Arrange
            var fakeProfileService = new FakeProfileService();
            var toBeAddedProfiles = new[]
            {
                new WSProfile() { Name = "Prof A" },
                new WSProfile() { Name = "Prof B" }
            };
            fakeProfileService.AddProfiles(toBeAddedProfiles);
            var repo = new ProfileRepository(fakeProfileService);

            // Act
            var actual = repo.DeleteProfile("Prof A");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(toBeAddedProfiles.Length - 1, fakeProfileService.AllProfiles.Length);
        }

        [TestMethod]
        public void RepositoryDeleteAllProfilesTest()
        {
            // Arrange
            var fakeProfileService = new FakeProfileService();
            var toBeAddedProfiles = new[]
            {
                new WSProfile() { Name = "Prof A" },
                new WSProfile() { Name = "Prof B" },
                new WSProfile() { Name = "Prof C" }
            };
            fakeProfileService.AddProfiles(toBeAddedProfiles);
            var repo = new ProfileRepository(fakeProfileService);
            Assert.AreEqual(3, fakeProfileService.AllProfiles.Length);

            // Act
            var actual = repo.DeleteAllProfiles();

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual(0, fakeProfileService.AllProfiles.Length);
        }
    }
}
