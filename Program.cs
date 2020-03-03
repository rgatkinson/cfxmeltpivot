using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CfxMeltPivot
    {
    class Program
        {
        public ProgramOptions ProgramOptions = new ProgramOptions();

        private FileStream fsOut = null;
        private TextWriter writer = null;

        private void OpenOutputWhile(FileMode fileMode, Action action)
            {
            try { 
                // Open the output file, creating if necessary, but appending if not
                fsOut = new FileStream(ProgramOptions.OutputFileName, fileMode, FileAccess.Write, FileShare.Read);
                writer = new StreamWriter(fsOut);

                action.Invoke();
                }
            finally
                {
                writer?.Flush();
                fsOut?.Flush();
                writer?.Dispose();
                fsOut?.Dispose();
                }
            }

        private void ReadFile(string experimentName, string fileName, int firstRow)
            {
            // Open the input file
            using FileStream fsIn = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using TextReader reader = new StreamReader(fsIn);

            // Burn the first line, since it has headers
            reader.ReadLine();

            // Loop until there's nothing more to read
            for (;;)
                {
                string line = reader.ReadLine();
                if (line == null)
                    break;

                // Split by comma or tab (we're not fussy here)
                string[] columns = line.Split(new char[] { ',', '\t' });

                IEnumerator<string> strings = ((IEnumerable<string>)columns).GetEnumerator();
                strings.MoveNext();

                // First column may be blank (due to how CFX Maestro exports); burn it if so
                if (columns[0].Length == 0)
                    {
                    strings.MoveNext();
                    }

                // Emit the columns, one per line, to the output
                NextDouble(strings, out double temp);
                for (int index=1;;index++)
                    {
                    if (!NextDouble(strings, out double value))
                        break;

                    int indexZ = index-1;
                    int row = indexZ / 12 + firstRow;
                    int column = indexZ % 12 + 1;

                    string output = $"{experimentName}\t{temp}\t{value}\t{WellName(row,column)}\t{index}\t{row}\t{column}";
                    writer.WriteLine(output);
                    }
                }
            }
        

        public void DoMain(string[] args)
            {
            ProgramOptions.Parse(args);

            if (ProgramOptions.Auto)
                {
                OpenOutputWhile(FileMode.Create, () =>
                    {
                    for (int i = 0; i < ProgramOptions.AutoFileNames.Count; i++)
                        {
                        ReadFile(ProgramOptions.AutoExperimentNames[i], ProgramOptions.AutoFileNames[i], ProgramOptions.AutoFirstRow[i]);
                        }
                    });
                }
            else
                {
                OpenOutputWhile(FileMode.Append, () =>
                    {
                    ReadFile(ProgramOptions.Experiment, ProgramOptions.InputFileName, ProgramOptions.FirstRow);
                    });
                }
            }

        private string WellName(int row, int column)
            {
            StringBuilder result = new StringBuilder();
            result.Append((char)(row-1+'A'));
            result.Append(column.ToString());
            return result.ToString();
            }

        private bool NextDouble(IEnumerator<string> strings, out double result)
            {
            string value = strings.Current;
            result = double.Parse(value);
            return strings.MoveNext();
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }
        }
    }
