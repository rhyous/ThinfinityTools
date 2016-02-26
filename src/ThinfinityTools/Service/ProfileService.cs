using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    public class ProfileService : IProfileService
    {
        private WSThinRDPClient _Client;
        internal WSThinRDPClient Client => _Client ?? (_Client = BuildClient());

        public WSProfile[] AllProfiles => Client.GetAllProfiles();

        public WSProfile[] AddProfiles(WSProfile[] profiles)
        {
            var createdProfiles = profiles.Select(wsProfile => Client.CreateProfile(wsProfile)).ToArray();
            Client.Commit();
            return createdProfiles;
        }

        public int DeleteProfile(string name)
        {
            return DeleteProfiles(AllProfiles.Where(profile => profile.Name == name));
        }

        public int DeleteAllProfiles()
        {
            return DeleteProfiles(AllProfiles);
        }

        private int DeleteProfiles(IEnumerable<WSProfile> profiles)
        {
            int deleteCount = 0;
            foreach (var profile in profiles)
            {
                Client.DeleteProfile(profile.ID);
                deleteCount++;
            }
            if (deleteCount > 0)
            {
                Client.Commit();
            }
            return deleteCount;
        }

        private static WSThinRDPClient BuildClient()
        {
            var client = new WSThinRDPClient();
            if (ConfigurationManager.AppSettings.Get("TrustAnyCert", false))
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            return client;
        }
    }
}
