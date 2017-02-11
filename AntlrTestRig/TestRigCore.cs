using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;

namespace AntlrTestRig
{
    class TestRigCore
    {
        private Dictionary<IToken, RecognitionException> _tokenExceptions;
        private Dictionary<ParserRuleContext, IToken> _contextTokenMapping;
        private readonly IDictionary<int, string> ruleNames = new Dictionary<int, string>();
        public const String LEXER_START_RULE_NAME = "tokens";

        public void Process(Assembly[] scanAssemblies, string content, AppArgs appArg)
        {
            var input = new AntlrInputStream(content);
            var allTypes = scanAssemblies.SelectMany(x=>x.GetTypes()).ToArray();
            var lexerType = allTypes
                .FirstOrDefault(x => x.BaseType == typeof(Lexer) && x.Name.Equals(appArg.GrammarName + "lexer", StringComparison.CurrentCultureIgnoreCase));
            if (lexerType == null)
                throw new Exception(string.Format("Lexer {0} not found.", appArg.GrammarName));
            var lexer = (Lexer)Activator.CreateInstance(lexerType, new object[] { input });
            var commonTokenStream = new CommonTokenStream(lexer);
            commonTokenStream.Fill();

            if (appArg.ShowTokens)
            {
                foreach (var token in commonTokenStream.GetTokens())
                {
                    Console.WriteLine(((CommonToken) token).ToString());
                }
            }

            if (appArg.StartRuleName == LEXER_START_RULE_NAME) return;


            var parserType = allTypes
                .FirstOrDefault(x => x.BaseType == typeof(Parser) && x.Name.Equals(appArg.GrammarName + "parser", StringComparison.CurrentCultureIgnoreCase));
            if (parserType == null)
                throw new Exception(string.Format("Parser {0} not found.", appArg.GrammarName));
            var parser = (Parser)Activator.CreateInstance(parserType, new object[] { commonTokenStream });



            if (appArg.Diagnoctics)
            {
                parser.AddErrorListener(new DiagnosticErrorListener());
                parser.Interpreter.PredictionMode = PredictionMode.LL_EXACT_AMBIG_DETECTION;
            }

            if (appArg.ShowTree || appArg.ShowGui)
            {
                parser.BuildParseTree = true;
            }

            if (appArg.SLL)
            { // overrides diagnostics
                parser.Interpreter.PredictionMode = (PredictionMode.SLL);
            }

            parser.Trace = appArg.Trace;

            var errorListener = new MyErrorListener();
            parser.AddErrorListener(errorListener);

            _tokenExceptions = errorListener.TokenExceptionMapping;
            _contextTokenMapping = errorListener.ContextTokenMapping;

            MethodInfo rootMethod = parserType.GetMethod(appArg.StartRuleName);
            IParseTree rootContext;
            try
            {
                rootContext = (IParseTree)rootMethod.Invoke(parser, new object[] { });
            }
            catch (TargetInvocationException err)
            {
                throw err;
            }

            foreach (var item in parser.RuleIndexMap)
            {
                ruleNames.Add(item.Value, item.Key);
            }

 
            if (appArg.ShowTree)
            {
                Console.WriteLine(rootContext.ToStringTree(parser));
            }

            if (appArg.ShowGui)
            {
                var model = GetDisplayNodeFromParseTree(rootContext);
                MainWindow mainWindow = new MainWindow(model);
                mainWindow.ShowDialog();
            }
        }
        private DisplayNode GetDisplayNodeFromParseTree(IParseTree node)
        {
            var model = new DisplayNode();

            if (node is TerminalNodeImpl)
            {
                var tNode = node as TerminalNodeImpl;
                model.String = tNode.GetText();
                model.IsToken = true;
                if (_tokenExceptions.ContainsKey(tNode.Symbol) || tNode.Symbol.TokenIndex == -1)
                {
                    model.HasError = true;
                }
                
            }
            else
            {
                var ruleNode = node as ParserRuleContext;

                //Add token to context if context does not have child,
                //This handels the extraneous token
                if (_contextTokenMapping.ContainsKey(ruleNode))
                {
                    var token = _contextTokenMapping[ruleNode];
                    model.HasError = true;
                    if (token.Type != -1 && node.ChildCount == 0)
                    {
                        var errorTokenNode = new DisplayNode();
                        errorTokenNode.HasError = true;
                        errorTokenNode.IsToken = true;
                        errorTokenNode.String = token.Text;
                        model.Children.Add(errorTokenNode);
                    }
                 
                }
         
                model.String = ruleNames[ruleNode.RuleIndex];
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                var subAstNode = GetDisplayNodeFromParseTree(node.GetChild(i));
                model.Children.Add(subAstNode);
            }

            return model;
        }
    }
}