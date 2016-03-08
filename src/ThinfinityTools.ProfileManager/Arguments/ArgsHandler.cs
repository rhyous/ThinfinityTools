using System;
using System.Collections.Generic;
using System.IO;
using SimpleArgs;
using ThinfinityTools.Thinfinity.WebApi;

namespace ThinfinityTools.ProfileManager.Arguments
{
    /// <summary>
    /// A class that implements IArgumentsHandler where command line
    /// arguments are defined.
    /// </summary>
    public sealed class ArgsHandler : ArgsHandlerBase
    {
        public override int MinimumRequiredArgs => 1;

        public ArgsHandler()
        {
            Arguments = new List<Argument>
            {
                new Argument
                {
                    Name = "ShowProfiles",
                    ShortName = "p",
                    Description = "Show all profiles.",
                    Example = "/{name}",
                    AllowedValues = CommonAllowedValues.TrueFalse,
                    Action = value => Console.Write(new Xml(Serializer.SerializeToXml(Repo.AllProfiles)).PrettyXml)
                },
                new Argument
                {
                    Name = "AddProfiles",
                    ShortName = "add",
                    Description = "Add profiles from an xml.",
                    Example = @"{name}=c:\path\to\profiles.xml",
                    CustomValidation = value => File.Exists(value),
                    Action = value =>
                    {
                        var addedProfiles = Repo.AddProfiles(Serializer.DeserializeFromXml<List<WSProfile>>(value));
                        var xml = Serializer.SerializeToXml(addedProfiles);
                        Console.WriteLine("The following profiles were created:{0}{1}", Environment.NewLine, xml);
                    }
                },
                new Argument
                {
                    Name = "DeleteProfile",
                    ShortName = "d",
                    Description = "Delete a profile by name.",
                    Example = @"{name}=profile-name",
                    Action = value => { Console.WriteLine("Deleted {0} profiles.", Repo.DeleteProfile(value)); }
                },
                new Argument
                {
                    Name = "DeleteAllProfiles",
                    ShortName = "da",
                    Description = "Delete all profiles. A an Xml of all profiles is saved.",
                    Example = @"/{name}",
                    AllowedValues = CommonAllowedValues.TrueFalse,
                    Action = value =>
                    {
                        Serializer.SerializeToXml("AllProfiles.xml");
                        Console.WriteLine("Deleted {0} profiles.", Repo.DeleteAllProfiles());
                    }
                },
                new Argument
                {
                    Name = "ProfileTemplate",
                    ShortName = "t",
                    Description = "An xml template of a profile. Use this with ProfileCsv to set default values.",
                    Example = @"{name}=template.xml",
                    CustomValidation = value => File.Exists(value)
                },
                new Argument
                {
                    Name = "GenerateTemplate",
                    ShortName = "gentem",
                    Description = "Generate an xml template of a profile.",
                    Example = @"{name}=outputTemplate.xml",
                    Action = value => Serializer.SerializeToXml(new WSProfile { Name = "Example 1", VirtualPath = "Example 1", Computer = "PC 1" }, value)
                },
                new Argument
                {
                    Name = "ProfileCsv",
                    ShortName = "csv",
                    Description = "A csv file to add a profile. Use with ProfileTemplate.",
                    Example = @"{name}=profileSettings.csv",
                    CustomValidation = value => File.Exists(value)
                },
                new Argument
                {
                    Name = "GenerateCsvTemplate",
                    ShortName = "gencsv",
                    Description = "Generate a template of a csv file to update a profile.",
                    Example = @"{name}=profileSettings.csv",
                    Action = value => { File.WriteAllLines(value, new SampleCsv().Lines); }
                },
                new Argument
                {
                    Name = "Out",
                    ShortName = "o",
                    Description = "The name of the output file for profiles added with ProfileCsv. If out=console, it goes to the screen.",
                    Example = @"{name}=AddedProfiles.xml",
                    DefaultValue = "AddedProfiles.xml"
                }
                // Add more args here
            };
        }

        public override void HandleArgs(IReadArgs inArgsHandler)
        {
            Handled = true;
            AddProfilesFromCsv();
        }

        public IProfileRepository Repo
        {
            get { return _Repo ?? (_Repo = new ProfileRepository(new ProfileService())); }
            set { _Repo = value; }
        }
        private IProfileRepository _Repo;

        public void AddProfilesFromCsv()
        {
            // Was Csv and Template provided
            var csvPath = Args.Value("ProfileCsv");
            var tempate = Args.Value("ProfileTemplate");
            if (!string.IsNullOrWhiteSpace(csvPath))
            {
                var csv = new Csv(csvPath);
                var profiles = csv.GetProfiles(tempate);
                if (profiles.Count > 0)
                {
                    var addedProfiles = Repo.AddProfiles(profiles);
                    Console.WriteLine("Added {0} profiles.", addedProfiles.Count);
                    if (Args.Value("Out").Equals("Console", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine(Serializer.SerializeToXml(addedProfiles));
                    }
                    else
                    {
                        Serializer.SerializeToXml(addedProfiles, Args.Value("Out"));
                    }
                }
            }
        }
    }
}