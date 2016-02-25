using System.Collections.Generic;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    interface IThinfinityRepository
    {
        List<WSProfile> AllProfiles { get; }
        void AddProfiles(IEnumerable<WSProfile> profiles);
        void DeleteProfile(string name);
    }
}
