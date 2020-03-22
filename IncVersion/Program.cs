using System;
using System.IO;
using System.Reflection;

namespace IncVersion
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
    class Program
    {
        private const string INDENT = "     ";

        static int Main(string[] args)
        {
            if(args.Length == 0)
            {
                ShowUsage();

                return 0;
            }

            var incrFileVersion = new AssemblyFileVersionIncrementer();

            try
            {
                incrFileVersion.IncrementAssemblyFileVersion(args[0]);
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
            Console.WriteLine($"{INDENT}Property <FileVersionAttribute> in {incrFileVersion.CsprojFileName} successfully changed from {incrFileVersion.OldVersion} to {incrFileVersion.NewVersion}.");
            Console.WriteLine($"{INDENT}These changes are applied to the compiled assembly at next build.");
            Console.ResetColor();
            Console.WriteLine();
            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine();

            Console.WriteLine($"Usage: {Assembly.GetExecutingAssembly().GetName().Name} Path_to_the_csproj_file");
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
