using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raw2sqlite
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        private string outputFile;
        [Option('o', "out",
           HelpText = "Output .sqlite file name")]
        public string OutputFile
        {
            get => outputFile ?? Path.ChangeExtension(InputFile, ".sqlite");
            set => outputFile = value;
        }

        private string inputFile;

        [Value(0, MetaName = "input file",
            HelpText = "Input .raw file name",
            Required = true)]
        public string InputFile
        {
            get => inputFile ?? string.Empty;
            set => inputFile = value;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var res = Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts)
                   );
            Console.WriteLine(res.Value);
        }

        private static int RunOptionsAndReturnExitCode(Options opts)
        {
            var rp = new RawProcessor(opts.InputFile, opts.OutputFile);
            Task.WaitAll(rp.Process());
            return rp.ExitCode;
        }
    }
}
