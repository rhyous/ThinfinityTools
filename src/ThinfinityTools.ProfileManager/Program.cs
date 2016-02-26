using SimpleArgs;
using ThinfinityTools.ProfileManager.Arguments;

namespace ThinfinityTools.ProfileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            ArgsManager.Instance.Start(new ArgsHandler(), args);
        }
    }
}
