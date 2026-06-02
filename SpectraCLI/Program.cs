using System;
using System.IO;
using System.IO.Pipes;

namespace SpectraCLI
{
    class Program
    {
        private const string PipeName = "SpectraControl";

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            string command = string.Join(" ", args).Trim();

            try
            {
                using (var pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut))
                {
                    pipe.Connect(2000);
                    using (var writer = new StreamWriter(pipe) { AutoFlush = true })
                    using (var reader = new StreamReader(pipe))
                    {
                        writer.WriteLine(command);
                        string response = reader.ReadLine() ?? "";
                        Console.WriteLine(response);
                        return response.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                    }
                }
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("ERROR: Spectra is not running.");
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: " + ex.Message);
                return 3;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Spectra CLI — Control Spectra from the command line.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  SpectraCLI status");
            Console.WriteLine("  SpectraCLI get-vibrance");
            Console.WriteLine("  SpectraCLI set-vibrance <level>");
            Console.WriteLine("  SpectraCLI get-gpu");
            Console.WriteLine("  SpectraCLI ping");
            Console.WriteLine();
            Console.WriteLine("Spectra must be running for commands to work.");
        }
    }
}
