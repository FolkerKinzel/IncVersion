using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IncVersion
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Keine allgemeinen Ausnahmetypen abfangen", Justification = "<Ausstehend>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
    public class AssemblyFileVersionIncrementer
    {
        public Version? OldVersion { get; private set; }
        public Version? NewVersion { get; private set; }
        public string? CsprojFileName { get; internal set; }

        public void IncrementAssemblyFileVersion(string fileName)
        {
            string pathToCsproj;

            try
            {
                pathToCsproj = Path.GetFullPath(fileName);
            }
            catch (Exception e)
            {
                throw new ArgumentException("The first argument has to be a valid file path!", e);
            }

            if(!StringComparer.OrdinalIgnoreCase.Equals(".CSPROJ", Path.GetExtension(pathToCsproj)))
            {
                throw new ArgumentException($"The argument \"{pathToCsproj}\"{Environment.NewLine}is not a path to a csproj-file!");
            }

            if(!File.Exists(pathToCsproj))
            {
                throw new OperationFailedException($"File {pathToCsproj} not found.");
            }

            this.CsprojFileName = Path.GetFileName(pathToCsproj);

            XName FileVersion = "FileVersion";

            XElement csproj;

            try
            {
                csproj = System.Xml.Linq.XElement.Load(pathToCsproj);
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

            if(fileVersion is null)
            {
                //throw new OperationFailedException(
                //    $"There is no <FileVersion>-Property in {CsprojFileName}. Open the project file and add <FileVersion>1.0.0.0</FileVersion> to the first <PropertyGroup>.");

                fileVersion = new XElement(FileVersion)
                {
                    Value = "1.0.0.0"
                };

                propertyGroups.First().Add(fileVersion);
            }

            try
            {
                this.OldVersion = new Version(fileVersion.Value);
                this.NewVersion = new Version(OldVersion.Major, OldVersion.Minor, OldVersion.Build + 1, Math.Max(0, OldVersion.Revision));
                fileVersion.Value = NewVersion.ToString(4);
            }
            catch(Exception e)
            {
                throw new OperationFailedException("Can't parse <FileVersion> as System.Version.", e);
            }

            try
            {
                File.WriteAllText(pathToCsproj, csproj.ToString());
            }
            catch(Exception e)
            {
                throw new OperationFailedException($"Can't write changes to {CsprojFileName}.", e);
            }
        }
    }
}
