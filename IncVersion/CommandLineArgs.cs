using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IncVersion
{
    public class CommandLineArgs
    {
        public const string REVISION_INCREMENT_ARGUMENT = "--revision";

        public CommandLineArgs(string[] args)
        {
            if(args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length == 0)
            {
                throw new ArgumentException("Not enough command line arguments.");
            }

            try
            {
                FilePath = Path.GetFullPath(args[0]);
            }
            catch (Exception e)
            {
                throw new ArgumentException("The first argument has to be a valid file path!", e);
            }

            string ext = Path.GetExtension(FilePath);

            if (!ext.StartsWith('.') || !ext.EndsWith("PROJ", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"The argument \"{FilePath}\"{Environment.NewLine}is not a path to a Visual Studio project file!");
            }

            if(args.Length == 2)
            {
                if(StringComparer.OrdinalIgnoreCase.Equals(args[1], REVISION_INCREMENT_ARGUMENT))
                {
                    IncrementRevision = true;
                }
                else
                {
                    throw new ArgumentException("Unrecognized command line argument.", args[1]);
                }
            }

        }

        public string FilePath { get; }

        public bool IncrementRevision { get; }
    }
}
