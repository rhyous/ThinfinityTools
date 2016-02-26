using System.Collections.Generic;
using System.Linq;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IProfileService _Service;
        public ProfileRepository(IProfileService service)
        {
            _Service = service;
        }

        public List<WSProfile> AllProfiles => _Service.AllProfiles.ToList();
        public List<WSProfile> AddProfiles(IEnumerable<WSProfile> profiles)
        {
            return _Service.AddProfiles(profiles.ToArray()).ToList();
        }

        public int DeleteProfile(string name)
        {
            return _Service.DeleteProfile(name);
        }

        public int DeleteAllProfiles()
        {
            return _Service.DeleteAllProfiles();
        }
    }
}
