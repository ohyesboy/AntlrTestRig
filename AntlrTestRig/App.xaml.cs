using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AntlrTestRig
{
    class AppArgs
    {
        public string GrammarName;
        public string StartRuleName;
        public bool ShowGui;
        public bool ShowTokens;
        public bool ShowTree;
        public bool Trace;
        public bool SLL;
        public bool Diagnoctics;
        public string Encoding;
        public List<string> InputFiles = new List<string>();
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            var appArg = ReadOptionsFromArgs(args);
            if (appArg == null)
                ShowErrorAndExit(null);

            string input = ReadInputFromFileOrConsole(appArg.InputFiles.FirstOrDefault(), appArg.Encoding);

            try
            {
                var assemblies = Directory.GetFiles(Environment.CurrentDirectory, "*.dll")
                    .Select(x=> Assembly.LoadFile(x)).ToArray();
            
                new TestRigCore().Process(
                    assemblies,
                    input,
                    appArg);
            }
            catch (Exception err)
            {
                ShowErrorAndExit(err.Message);
            }

            Environment.Exit(0);
        }

        private string ReadInputFromFileOrConsole(string fileName, string encodingName)
        {
            if (fileName == null)
            {
                Console.WriteLine("Enter the input, end with Ctrl+Z and Enter");
                StringBuilder sb = new StringBuilder();
                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
            else
            {
                var encoding = Encoding.Default;
                if (encodingName != null)
                    encoding = Encoding.GetEncoding(encodingName);
                return File.ReadAllText(fileName, encoding);
            }

        }

        private AppArgs ReadOptionsFromArgs(string[] args)
        {
            var appArg = new AppArgs();
            if (args.Length < 3)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("AntlrTestRig.exe GrammarName startRuleName\n" +
                                   "  [-tokens] [-tree] [-gui] [-ps file.ps] [-encoding encodingname]\n" +
                                   "  [-trace] [-diagnostics] [-SLL]\n" +
                                   "  [input-filename(s)]");
                sb.AppendLine("Use startRuleName='tokens' if GrammarName is a lexer grammar.");
                sb.AppendLine("Omitting input-filename makes rig read from stdin, end with Ctrl+Z and Enter");
                Console.WriteLine(sb.ToString());
                return null;
            }

            int i = 1; //starts with 1, the 0 is the app file itself
            appArg.GrammarName = args[i];
            i++;
            appArg.StartRuleName = args[i];
            i++;
            while (i < args.Length)
            {
                String arg = args[i];
                i++;
                if (arg[0] != '-')
                { // input file name
                    appArg.InputFiles.Add(arg);
                    continue;
                }
                if (arg.Equals("-tree"))
                {
                    appArg.ShowTree = true;
                }
                if (arg.Equals("-gui"))
                {
                    appArg.ShowGui = true;
                }
                if (arg.Equals("-tokens"))
                {
                    appArg.ShowTokens = true;
                }
                else if (arg.Equals("-trace"))
                {
                    appArg.Trace = true;
                }
                else if (arg.Equals("-SLL"))
                {
                    appArg.SLL = true;
                }
                else if (arg.Equals("-diagnostics"))
                {
                    appArg.Diagnoctics = true;
                }
                else if (arg.Equals("-encoding"))
                {
                    if (i >= args.Length)
                    {
                        Console.WriteLine("missing encoding on -encoding");
                        return null;
                    }
                    appArg.Encoding = args[i];
                    i++;
                }
            }

            return appArg;
        }

        private void ShowErrorAndExit(string msg)
        {
           Console.WriteLine(msg);
           Environment.Exit(0);
        }
    }
}
