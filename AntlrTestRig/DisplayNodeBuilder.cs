using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AntlrTestRig
{
    class DisplayNodeBuilder
    {
        private Dictionary<IToken, RecognitionException> _tokenExceptions;
        private Dictionary<ParserRuleContext, IToken> _contextTokenMapping;
        private string[] _ruleNames;

        public DisplayNodeBuilder(Dictionary<IToken, RecognitionException> tokenExceptions,
            Dictionary<ParserRuleContext, IToken> contextTokenMapping,
            string[] ruleNames)
        {
            _tokenExceptions = tokenExceptions;
            _contextTokenMapping = contextTokenMapping;
            _ruleNames = ruleNames;

        }


        public DisplayNode GetDisplayNodeFromParseTree(IParseTree node, bool showRuleIndex)
        {
            var model = new DisplayNode();

            if (node is TerminalNodeImpl)
            {
                var tNode = node as TerminalNodeImpl;
                model.String = tNode.GetText();
                if (showRuleIndex)
                    model.String += " = " + tNode.Symbol.Type;
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

                model.String = _ruleNames[ruleNode.RuleIndex];
                if (showRuleIndex)
                    model.String += " = " + ruleNode.RuleIndex;
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                var subAstNode = GetDisplayNodeFromParseTree(node.GetChild(i), showRuleIndex);
                model.Children.Add(subAstNode);
            }

            return model;
        }
    }
}