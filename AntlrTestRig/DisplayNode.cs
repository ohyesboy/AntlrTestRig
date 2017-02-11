using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace AntlrTestRig
{
    public class DisplayNode
    {
        public double Width; //This cinludes the string width and padding
        public double Middle;
        public double Top;

        public string String;
        public bool IsToken;
        public List<DisplayNode> Children = new List<DisplayNode>();
        public bool HasError; 

        public override string ToString()
        {
            return String;
        }
    }
}
