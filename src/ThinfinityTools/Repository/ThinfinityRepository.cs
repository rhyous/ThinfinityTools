using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using ThinfinityTools.Thinfinity.WebApi;



namespace ThinfinityTools
{
    public class ThinfinityRepository : IThinfinityRepository
    {
        private IThinfinityService _Service;
        public ThinfinityRepository(IThinfinityService service)
        {
            _Service = service;
        }

        public List<WSProfile> AllProfiles => _Service.AllProfiles.ToList();
        public void AddProfiles(IEnumerable<WSProfile> profiles)
        {
            _Service.AddProfiles(profiles.ToArray());
        }

        public void DeleteProfile(string name)
        {
            _Service.DeleteProfile(name);
        }
    }
}
