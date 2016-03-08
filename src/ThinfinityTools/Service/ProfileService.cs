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
            WSProfile[] createdProfiles = new WSProfile[profiles.Length];
            for (int i = 0; i < profiles.Length; i++)
            {
                var wsProfile = profiles[i];
                createdProfiles[i] = Client.CreateProfile(wsProfile);
            }
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
