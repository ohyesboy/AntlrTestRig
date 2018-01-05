using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrTestRig
{
    class AppArgHelper
    {
        public static string ReadInputFromFileOrConsole(string fileName, string encodingName)
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
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var textReader = new StreamReader(fileStream, encoding))
                {
                    var content = textReader.ReadToEnd();
                    return content;
                }

            }

        }

        public static AppArgs ReadOptionsFromArgs(string[] args)
        {
            var appArg = new AppArgs();
            if (args.Length < 3)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("AntlrTestRig.exe GrammarName startRuleName\n" +
                                   "  [-tokens] [-tree] [-gui] [-ps file.ps] [-encoding encodingname]\n" +
                                   "  [-trace] [-diagnostics] [-SLL]\n" +
                                   "  [-folder]\n" +
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
                    appArg.InputFile = arg;
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

                else if (arg.Equals("-folder"))
                {
                    if (i >= args.Length)
                    {
                        Console.WriteLine("missing encoding on -folder");
                        return null;
                    }
                    appArg.TargetFolder = args[i];
                    i++;
                }
                else if (arg.Equals("-showtype"))
                {
                    appArg.ShowType = true;
                }
            }

            return appArg;
        }
    }
}
