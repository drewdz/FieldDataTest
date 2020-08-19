using DataFactory.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FieldDataTest
{
    static class Program
    {
        private static IEnumerable<EventData> _Data;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //if (args == null)
            //{
            //    Debug.WriteLine("No file name in args.");
            //    return;
            //}

            //var file = args[0];
            //_Data = ImportData(file);
            //if ((_Data == null) || (_Data.Count() == 0))
            //{
            //    Debug.WriteLine("Could not parse file data.");
            //    return;
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm(_Data.ToList()));
            Application.Run(new MainForm());
        }

        static IEnumerable<EventData> ImportData(string file)
        {
            using (var sr = new StreamReader(file))
            {
                var line = string.Empty;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    var d = EventData.FromCsv(line);
                    if (d == null)
                    {
                        Debug.WriteLine($"Could not parse CSV: {line}");
                    }
                    else
                    {
                        yield return d;
                    }
                }
            }
        }
    }
}
