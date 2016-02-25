using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    public interface IThinfinityService
    {
        WSProfile[] AllProfiles { get; }
        void AddProfiles(WSProfile[] profiles);
        void DeleteProfile(string name);
    }
}
