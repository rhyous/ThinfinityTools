using System.Collections.Generic;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    public interface IProfileRepository
    {
        List<WSProfile> AllProfiles { get; }
        List<WSProfile> AddProfiles(IEnumerable<WSProfile> profiles);
        int DeleteProfile(string name);
        int DeleteAllProfiles();
    }
}
