using SimpleArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools.ProfileManager.Arguments
{
    // Add this line of code to Main() in Program.cs
    //
    //   ArgsManager.Instance.Start(new ArgsHandler(), args);
    //

    /// <summary>
    /// A class that implements IArgumentsHandler where command line
    /// arguments are defined.
    /// </summary>
    public sealed class ArgsHandler : ArgsHandlerBase
    {
        public ArgsHandler()
        {
            Arguments = new List<Argument>
            {
                new Argument
                {
                    Name = "ShowProfiles",
                    ShortName = "p",
                    Description = "I show all the profiles",
                    Example = "/{name}",
                    Action = (value) =>
                    {
                        var repo = new ThinfinityRepository(new ThinfinityService());
                        var text = Serializer.SerializeToXml(repo.AllProfiles);
                        var xml = new Xml(text);
                        Console.Write(xml.PrettyXml);
                    }
                },
                new Argument
                {
                    Name = "AddProfiles",
                    ShortName = "add",
                    Description = "I add profiles from an xml.",
                    Example = @"{name}=c:\path\to\profiles.xml",
                    CustomValidation = (value) => File.Exists(value),
                    Action = (value) =>
                    {
                        var profiles = Serializer.DeserializeFromXml<List<WSProfile>>(value);
                        var repo = new ThinfinityRepository(new ThinfinityService());
                        repo.AddProfiles(profiles);
                    }
                },
                new Argument
                {
                    Name = "DeleteProfile",
                    ShortName = "d",
                    Description = "I delete a profile.",
                    Example = @"{name}=profile-name",
                    Action = (value) =>
                    {
                        var repo = new ThinfinityRepository(new ThinfinityService());
                        repo.DeleteProfile(value);
                    }
                },
                new Argument
                {
                    Name = "ProfileTemplate",
                    ShortName = "t",
                    Description = "I am a template of a profile.",
                    Example = @"{name}=template.xml",
                    CustomValidation = (value) => File.Exists(value)
                },
                new Argument
                {
                    Name = "ProfileCsv",
                    ShortName = "csv",
                    Description = "I am a csv file to update a profile created from a template.",
                    Example = @"{name}=profileSettings.csv",
                    CustomValidation = (value) => File.Exists(value)
                }
                // Add more args here
            };
        }

        public override void HandleArgs(IReadArgs inArgsHandler)
        {
            Handled = true;

            // Was Csv and Template provided
            var csv = Args.Value("ProfileCsv");
            var tempate = Args.Value("ProfileTemplate");
            if (!string.IsNullOrWhiteSpace(csv) && !string.IsNullOrWhiteSpace(tempate))
            {
                // Read CSV
                var lines = File.ReadAllLines(csv);
                var headers = new List<string>();
                var rows = new List<List<string>>();
                var profiles = new List<WSProfile>();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0)
                    {
                        headers.AddRange(lines[i].Split(',').Select(s => s.Trim()));
                        continue;
                    }
                    rows.Add(lines[i].Split(',').Select(s => s.Trim()).ToList());
                }

                foreach (var row in rows)
                {
                    var profile = Serializer.DeserializeFromXml<WSProfile>(tempate);
                    for (int i = 0; i < headers.Count; i++)
                    {
                        var type = profile.GetType();
                        var prop = type.GetProperty(headers[i]);
                        prop.SetValue(profile, row[i], null);
                    }
                    profiles.Add(profile);
                }
                if (profiles.Count > 0)
                {
                    var repo = new ThinfinityRepository(new ThinfinityService());
                    repo.AddProfiles(profiles);
                }
            }

        }
    }
}
