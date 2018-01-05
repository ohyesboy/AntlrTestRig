using System;

namespace AntlrTestRig
{
    [Serializable]
    public class AppArgs
    {
        public string GrammarName;
        public string StartRuleName;
        public bool ShowGui;
        public bool ShowType;
        public bool ShowTokens;
        public bool ShowTree;
        public bool Trace;
        public bool SLL;
        public bool Diagnoctics;
        public string Encoding;
        public string InputFile;
        //Grammar dll folder, can be absolute or relative (to current folder),
        //if not set, will use current folder.
        public string Folder; 
    }
}