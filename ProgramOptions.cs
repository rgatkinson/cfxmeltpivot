using Mono.Options;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace CfxMeltPivot
    {
    class ProgramOptions
        {
        public List<string> AutoFileNames = new List<string>
            {
            "E19103101 -  Melt Curve RFU Results_SYBR.csv", 
            "E19110401 -  Melt Curve RFU Results_SYBR.csv",
            "E19110701 -  Melt Curve RFU Results_SYBR.csv",
            "E19111501 -  Melt Curve RFU Results_SYBR.csv",
            "E19111801 -  Melt Curve RFU Results_SYBR.csv",
            "E19111901 -  Melt Curve RFU Results_SYBR.csv",
            "E19120901 -  Melt Curve RFU Results_SYBR.csv",
            "E20012701a-reread -  Melt Curve RFU Results_SYBR.csv",
            "E20012701b-reread -  Melt Curve RFU Results_SYBR.csv",
            "E20020401a-reread -  Melt Curve RFU Results_SYBR.csv",
            "E20020401b-reread -  Melt Curve RFU Results_SYBR.csv",
            "E20030501a -  Melt Curve RFU Results_SYBR.csv",
            "E20030501b -  Melt Curve RFU Results_SYBR.csv",
            };

        public List<string> AutoExperimentNames = new List<string>
            {
            "E19103101", "E19110401", "E19110701", "E19111501", "E19111801", "E19111901", "E19120901", 
            "E20012701", "E20012701", 
            "E20020401", "E20020401",
            "E20030501", "E20030501",
            };

        public List<int> AutoFirstRow = new List<int>
            {
            1, 1, 1, 1, 1, 1, 1, 
            1, 9, 
            1, 9,
            1, 9,
            };

        public bool         Auto = true;
        public List<string> ExtraOptions = new List<string>();
        public string       InputFileName = null;
        public bool         ShowUsage = false;
        public bool         Verbose = false;
        public string       Experiment = "E20030101";
        public int          FirstRow = 1;
        
        public string OutputFileRoot = "FluorescenceByIndex";
        public string OutputExtension = ".txt";
        public string OutputFileName => OutputFileRoot + OutputExtension;

        public IndentedTextWriter StdOut;
        public IndentedTextWriter StdErr;
        public string IndentString = "   ";

        private OptionSet Options;

        public ProgramOptions()
            {
            Options = new OptionSet
                {
                    { "a|auto",         $"automatically set options", (string a) => Auto = a != null },
                    { "e=|experiment=", $"the name of the experiment in question", (string s) => Experiment = s, Auto = false },
                    { "f=|file=",       $"the name of the exported CFX file to parse", (string f) => InputFileName = f, Auto = false},
                    { "r=|row=",        $"first row", (int row) => { FirstRow = row; Auto = false; } },
                    { "o=|output=",     $"root name of the output file (default=\"{OutputFileRoot}\"). Always of type {OutputExtension}", s => OutputFileRoot = s },
                    { "h|help|?",       $"show this message and exit", (string h) => ShowUsage = h != null },
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
            IndentedTextWriter writer = StdErr;

            if (e != null)
                {
                writer.WriteLine(e.Message);
                }

            writer.WriteLine("Options:");
            Options.WriteOptionDescriptions(writer);
            writer.WriteLine();
            }
        }
    }
