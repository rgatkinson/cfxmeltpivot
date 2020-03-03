using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CfxMeltPivot
    {
    class Program
        {
        public ProgramOptions ProgramOptions = new ProgramOptions();

        public void DoMain(string[] args)
            {
            ProgramOptions.Parse(args);

            // Open the output file, creating if necessary, but appending if not
            using var fs = new FileStream(ProgramOptions.OutputFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }
        }
    }
