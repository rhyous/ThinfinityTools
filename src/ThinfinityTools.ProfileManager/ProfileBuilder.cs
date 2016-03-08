using System.Collections.Generic;
using System.Data;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools.ProfileManager
{
    public static class ProfileBuilder
    {
        public static List<WSProfile> GetProfiles(this Csv csv, string template = null)
        {
            var profiles = new List<WSProfile>();
            foreach (DataRow row in csv.Table.Rows)
            {
                var profile = !string.IsNullOrWhiteSpace(template)
                    ? Serializer.DeserializeFromXml<WSProfile>(template)
                    : new WSProfile();
                foreach (var header in csv.Headers)
                {
                    var type = profile.GetType();
                    var prop = type.GetProperty(header);
                    prop.SetValue(profile, row[header], null);
                }
                profiles.Add(profile);
            }
            return profiles;
        }
    }
}
