using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AntlrTestRig
{
    public class InfoCollectorVisitor : IParseTreeListener
    {
        Dictionary<ParserRuleContext, IToken> ContextTokenMapping;
        public InfoCollectorVisitor(Dictionary<ParserRuleContext, IToken> ContextTokenMapping)
        {
            this.ContextTokenMapping = ContextTokenMapping;
        }
        public ParserRuleContext LastContext;
        public ParserRuleContext LastErrorContext;
        public void VisitTerminal(ITerminalNode node)
        {
           
        }

        public void VisitErrorNode(IErrorNode node)
        {
           
        }

        public void EnterEveryRule(ParserRuleContext ctx)
        {
            LastContext = ctx;
            if (ctx.exception != null || ContextTokenMapping.ContainsKey(ctx))
                LastErrorContext = ctx;
      
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
           
        }
    }
}