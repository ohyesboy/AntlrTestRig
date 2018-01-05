using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;

namespace AntlrTestRig
{
    /// <summary>
    /// A proxy live in a new domain can access loaded dlls and be unloaded along the new domain
    /// </summary>
    class Proxy : MarshalByRefObject
    {
        private List<string> _dllBlackList = new List<string>()
        {
            "Antlr4.Runtime.Standard.dll"
        };

        private Assembly[] _assemblies;
        private const String LEXER_START_RULE_NAME = "tokens";
    

        public void LoadDlls(string assemblyPath)
        {
            _assemblies = Directory.GetFiles(assemblyPath, "*.dll")
                .Where(x => (!_dllBlackList.Contains(Path.GetFileName(x))))
                .Select(x =>
                {

                    GC.Collect();// make sure no dll is locked by any object not collected.
                    byte[] assemblyBytes = null;
                    try
                    {
                        assemblyBytes = File.ReadAllBytes(x);
                    }
                    catch (IOException err)
                    {
                        Console.WriteLine(err.Message);
                    }


                    Assembly assembly = Assembly.Load(assemblyBytes);

                    return assembly;

                }).ToArray();

        }

        public DisplayNode ProcessInput(string inputText, AppArgs args)
        {
            var input = new AntlrInputStream(inputText);
            var allTypes = _assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var lexerType = allTypes
                .FirstOrDefault(x => x.BaseType == typeof(Lexer) && x.Name.Equals(args.GrammarName + "lexer", StringComparison.CurrentCultureIgnoreCase));
            if (lexerType == null)
                throw new ApplicationException(string.Format("Lexer {0} not found.", args.GrammarName));
            var lexer = (Lexer)Activator.CreateInstance(lexerType, new object[] { input });
            var commonTokenStream = new CommonTokenStream(lexer);
            commonTokenStream.Fill();

            if (args.ShowTokens)
            {
                foreach (var token in commonTokenStream.GetTokens())
                {
                    Console.WriteLine(((CommonToken)token).ToString());
                }
            }

            if (args.StartRuleName == LEXER_START_RULE_NAME) return null;


            var parserType = allTypes
                .FirstOrDefault(x => x.BaseType == typeof(Parser) && x.Name.Equals(args.GrammarName + "parser", StringComparison.CurrentCultureIgnoreCase));
            if (parserType == null)
                throw new ApplicationException(string.Format("Parser {0} not found.", args.GrammarName));
            var parser = (Parser)Activator.CreateInstance(parserType, new object[] { commonTokenStream });


            if (args.Diagnoctics)
            {
                parser.AddErrorListener(new DiagnosticErrorListener());
                parser.Interpreter.PredictionMode = PredictionMode.LL_EXACT_AMBIG_DETECTION;
            }

            if (args.ShowTree || args.ShowGui)
            {
                parser.BuildParseTree = true;
            }

            if (args.SLL)
            { // overrides diagnostics
                parser.Interpreter.PredictionMode = (PredictionMode.SLL);
            }

            parser.Trace = args.Trace;

            var errorListener = new MyErrorListener();
            parser.AddErrorListener(errorListener);

            MethodInfo rootMethod = parserType.GetMethod(args.StartRuleName);
            if (rootMethod == null)
                throw new ApplicationException($"Start rule \"{args.StartRuleName}\" does not exist");
            IParseTree rootContext;
            rootContext = (IParseTree)rootMethod.Invoke(parser, new object[] { });

            if (args.ShowTree)
            {
                Console.WriteLine(rootContext.ToStringTree(parser));
            }

            return new DisplayNodeBuilder(errorListener.TokenExceptionMapping, errorListener.ContextTokenMapping, parser.RuleNames)
                .GetDisplayNodeFromParseTree(rootContext, args.RuleIndex);

        }

    }
}