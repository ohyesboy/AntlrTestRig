using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace AntlrTestRig
{
    public class InfoCollectorVisitor : IParseTreeListener
    {
        public ParserRuleContext lastContext;
       
        public void VisitTerminal(ITerminalNode node)
        {
            //throw new NotImplementedException();
        }

        public void VisitErrorNode(IErrorNode node)
        {
            //throw new NotImplementedException();
        }

        public void EnterEveryRule(ParserRuleContext ctx)
        {
            lastContext = ctx;
            //throw new NotImplementedException();
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            //throw new NotImplementedException();
        }
    }
}