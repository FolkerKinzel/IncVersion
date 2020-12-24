using System;
using System.IO;
using System.Reflection;

namespace IncVersion
{
    class Program
    {
        private const string INDENT = "     ";

        private const int ERROR_INVALID_COMMAND_LINE = 0x667;

        static int Main(string[] args)
        {
            CommandLineArgs cmdArgs;

            try
            {
                cmdArgs = new CommandLineArgs(args);
            }
            catch(ArgumentException e)
            {
                WriteError(e);
                ShowUsage();
                return ERROR_INVALID_COMMAND_LINE;
            }

            var incrFileVersion = new AssemblyFileVersionIncrementer();

            try
            {
                incrFileVersion.IncrementAssemblyFileVersion(cmdArgs);
            }
            catch(OperationFailedException e)
            {
                WriteError(e);
                return -1;
            }
            catch(ArgumentException e)
            {
                WriteError(e);
                return -1;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name}: ");

            if (incrFileVersion.OldVersion == null)
            {
                Console.WriteLine($"{INDENT}Property <FileVersionAttribute> with Version {incrFileVersion.NewVersion} added to {incrFileVersion.CsprojFileName}.");
                Console.WriteLine($"{INDENT}These changes are applied to the compiled assembly at next build.");
            }
            else
            {
                Console.WriteLine($"{INDENT}Property <FileVersionAttribute> in {incrFileVersion.CsprojFileName} changed from {incrFileVersion.OldVersion} to {incrFileVersion.NewVersion}.");
                Console.WriteLine($"{INDENT}These changes are applied to the compiled assembly at next build.");
            }
            Console.ResetColor();
            Console.WriteLine();
            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} increments the the <FileVersion> property in the Visual Studio Project File automatically after every build.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("To increment the Build part of the <FileVersion> property:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{INDENT}dotnet {Assembly.GetExecutingAssembly().GetName().Name}.dll Path_to_the_csproj_file");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("To increment the Revision part of the <FileVersion> property:");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{INDENT}dotnet {Assembly.GetExecutingAssembly().GetName().Name}.dll Path_to_the_csproj_file {CommandLineArgs.REVISION_INCREMENT_ARGUMENT}");
            Console.ResetColor();
            Console.WriteLine("======");

            //Console.ReadLine();
        }


        private static void WriteError(Exception e)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} - Error: ");
            Console.WriteLine($"{INDENT}{e.Message}");
            Console.WriteLine();

            if (e.InnerException != null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(e.InnerException);
            }
            Console.ResetColor();
            //Console.ReadLine();

        }
    }
}
