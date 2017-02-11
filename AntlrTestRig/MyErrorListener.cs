using System.Collections.Generic;
using Antlr4.Runtime;

namespace AntlrTestRig
{
    class MyErrorListener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            TokenExceptionMapping[offendingSymbol] = e; //e may be null;
            var ruleContext = ((Parser)recognizer).Context;
            ContextTokenMapping[ruleContext] = offendingSymbol;
        }

        public Dictionary<ParserRuleContext, IToken> ContextTokenMapping = new Dictionary<ParserRuleContext, IToken>();

        public Dictionary<IToken, RecognitionException> TokenExceptionMapping = new Dictionary<IToken, RecognitionException>(); 
    }
}