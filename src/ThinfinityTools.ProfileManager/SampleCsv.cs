namespace ThinfinityTools.ProfileManager
{
    internal class SampleCsv
    {
        public string[] Lines => new[]
        {
            "Name, Computer, Users",
            @"Example 1, 10.0.0.1, domain\user1",
            @"Example 2, 10.0.0.2, domain\user2",
            @"Example 3, 10.0.0.3, domain\user3"
        };
    }
}
