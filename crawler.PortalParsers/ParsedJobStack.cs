using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.PortalParsers
{
    public class ParsedJobStack:Stack<ParsedJob>
    {        
        private static ParsedJobStack instance;

        public static ParsedJobStack Instance
        {
            get {
                if (instance == null)
                {
                    instance = new ParsedJobStack();
                }
                return instance;
            }
        }
        private ParsedJobStack(){  }

    }
}
