using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    public interface IProfileService
    {
        WSProfile[] AllProfiles { get; }
        WSProfile[] AddProfiles(WSProfile[] profiles);
        int DeleteProfile(string name);
        int DeleteAllProfiles();


    }
}
