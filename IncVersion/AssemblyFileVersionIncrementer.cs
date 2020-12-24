using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IncVersion
{
    public class AssemblyFileVersionIncrementer
    {
        public Version? OldVersion { get; private set; }
        public Version? NewVersion { get; private set; }
        public string? CsprojFileName { get; internal set; }

        public void IncrementAssemblyFileVersion(CommandLineArgs cmdArgs)
        {
            if(cmdArgs is null)
            {
                throw new ArgumentNullException(nameof(cmdArgs));
            }

            if(!File.Exists(cmdArgs.FilePath))
            {
                throw new OperationFailedException($"File {cmdArgs.FilePath} not found.");
            }

            this.CsprojFileName = Path.GetFileName(cmdArgs.FilePath);

            XName FileVersion = "FileVersion";

            XElement csproj;

            try
            {
                csproj = System.Xml.Linq.XElement.Load(cmdArgs.FilePath);
            }
            catch(Exception e)
            {
                throw new OperationFailedException($"Unable to parse {CsprojFileName} as XML.", e);
            }

            

            if(csproj.Attribute("Sdk") is null)
            {
                throw new OperationFailedException($"{CsprojFileName} is not a Sdk-style project file.");
            }

            var propertyGroups = csproj.Elements().Where(x => x.Name == "PropertyGroup");

            if(!propertyGroups.Any())
            {
                throw new OperationFailedException($"{CsprojFileName} contains no <PropertyGroup>.");
            }

            XElement? fileVersion = propertyGroups.FirstOrDefault(x => x.Elements().Any(x => x.Name == FileVersion))?.Element(FileVersion);

            if (fileVersion is null)
            {
                //throw new OperationFailedException(
                //    $"There is no <FileVersion>-Property in {CsprojFileName}. Open the project file and add <FileVersion>1.0.0.0</FileVersion> to the first <PropertyGroup>.");

                fileVersion = new XElement(FileVersion)
                {
                    Value = "1.0.0.0"
                };

                propertyGroups.First().Add(fileVersion);

                this.NewVersion = new Version(1,0,0,0);
            }
            else if (fileVersion.Value.Length == 0)
            {
                fileVersion.Value = "1.0.0.0";

                this.NewVersion = new Version(1,0,0,0);
                this.OldVersion = new Version();
            }
            else
            {
                try
                {
                    string?[] parts = fileVersion.Value.Split('-', 2, StringSplitOptions.None);

                    this.OldVersion = new Version(parts[0]!);

                    if (cmdArgs.IncrementRevision)
                    {
                        this.NewVersion = new Version(OldVersion.Major, OldVersion.Minor, Math.Max(0, OldVersion.Build), OldVersion.Revision + 1);
                    }
                    else
                    {
                        this.NewVersion = new Version(OldVersion.Major, OldVersion.Minor, OldVersion.Build + 1, Math.Max(0, OldVersion.Revision));
                    }

                    if (parts.Length == 2)
                    {
                        fileVersion.Value = $"{NewVersion.ToString(4)}-{parts[1]}";
                    }
                    else
                    {
                        fileVersion.Value = NewVersion.ToString(4);
                    }
                }
                catch (Exception e)
                {
                    throw new OperationFailedException("Can't parse <FileVersion> as System.Version.", e);
                }
            }

            try
            {
                File.WriteAllText(cmdArgs.FilePath, csproj.ToString());
            }
            catch(Exception e)
            {
                throw new OperationFailedException($"Can't write changes to {CsprojFileName}.", e);
            }
        }
    }
}
