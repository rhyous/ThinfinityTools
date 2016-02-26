using System.Collections.Generic;
using System.Linq;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools.Tests.Fakes
{
    public class FakeProfileService : IProfileService
    {
        private readonly List<WSProfile> _Profiles = new List<WSProfile>();
        public WSProfile[] AllProfiles => _Profiles.ToArray();

        public WSProfile[] AddProfiles(WSProfile[] profiles)
        {
            _Profiles.AddRange(profiles);
            return profiles;
        }

        public int DeleteProfile(string name)
        {
            var toBeDeleted = AllProfiles.Where(profile => profile.Name == name).ToList();
            int deleteCount = toBeDeleted.Count;
            foreach (var profile in toBeDeleted)
            {
                _Profiles.Remove(profile);
            }
            return deleteCount;
        }

        public int DeleteAllProfiles()
        {
            int deleteCount = AllProfiles.Length;
            _Profiles.Clear();
            return deleteCount;
        }
    }

}
