using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace CfxMeltPivot
    {
    class ProgramOptions
        {
        public List<String> ExtraOptions = new List<string>();
        public string       InputFileName = null;
        public bool         ShowUsage = false;
        public bool         Verbose = false;
        
        public string OutputFileRoot = "Fluorescence";
        public string OutputExtension = ".csv";
        public string OutputFileName => OutputFileRoot + OutputExtension;

        public IndentedTextWriter StdOut;
        public IndentedTextWriter StdErr;
        public string IndentString = "   ";

        private OptionSet Options;

        public ProgramOptions()
            {
            Options = new OptionSet
                {
                    { "f=|file=",  $"the name of the exported CFX file to parse", (string f) => InputFileName = f },
                    { "o=|output=", $"root name of the output file (default=\"{OutputFileRoot}\"). Always of type {OutputExtension}", s => OutputFileRoot = s },
                    { "h|help|?",  $"show this message and exit", (string h) => ShowUsage = h != null },
                };

            // In case we don't validate
            StdOut = MakeIndentedTextWriter(Console.Out);
            StdErr = MakeIndentedTextWriter(Console.Error);
            }

        public void Parse(string[] args)
            {
            try
                {
                ExtraOptions = Options.Parse(args);
                if (InputFileName == null && ExtraOptions.Count > 0)
                    {
                    InputFileName = ExtraOptions[0];
                    ExtraOptions.RemoveAt(0);
                    }
                if (ShowUsage)
                    {
                    Usage(null);
                    }
                Validate();
                
                StdOut = MakeIndentedTextWriter(Console.Out);
                StdErr = MakeIndentedTextWriter(Console.Error);
                }
            catch (OptionException e)
                {
                Usage(e);
                }
            }

        private IndentedTextWriter MakeIndentedTextWriter(TextWriter writer)
            {
            var result = new IndentedTextWriter(writer, IndentString);
            result.Indent = 0;
            return result;
            }


        private void Throw(string message, string optionName = null)
            {
            throw new OptionException(Options.MessageLocalizer($"Error: { message }"), optionName);
            }

        private void Validate()
            {
            if (ExtraOptions.Count > 0)
                {
                Throw($"{ ExtraOptions.Count } extra options given");
                }
            }

        private void Usage(OptionException e)
            {
            }
        }
    }
