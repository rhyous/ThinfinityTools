using System.Configuration;
using System.Net;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools
{
    public class ThinfinityService : IThinfinityService
    {
        private WSThinRDPClient _Client;
        internal WSThinRDPClient Client => _Client ?? (_Client = BuildClient());

        public WSProfile[] AllProfiles => Client.GetAllProfiles();

        public void AddProfiles(WSProfile[] profiles)
        {
            foreach (var wsProfile in profiles)
            {
                var response = Client.CreateProfile(wsProfile);
                Client.Commit();
            }
        }

        public void DeleteProfile(string name)
        {
            var deletedAtLeastOne = false;
            foreach (var profile in AllProfiles)
            {
                if (profile.Name == name)
                {
                    Client.DeleteProfile(profile.ID);
                    deletedAtLeastOne = true;
                }
            }
            if (deletedAtLeastOne)
            {
                Client.Commit();
            }
        }

        private WSThinRDPClient BuildClient()
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
