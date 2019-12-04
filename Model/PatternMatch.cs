using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyEveLayoutTool.Model
{
    class PatternMatch
    {
        public PatternMatch(string pattern, string error)
        {
            Pattern = pattern;
            ErrorText = error;
        }

        public string Pattern { get; set; }
        public string ErrorText { get; set; }
    }
}
