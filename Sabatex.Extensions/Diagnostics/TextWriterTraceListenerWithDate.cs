using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace sabatex.Extensions.Diagnostics
{
    public class TextWriterTraceListenerWithDate: TextWriterTraceListener
    {
        public TextWriterTraceListenerWithDate(string fileName) : base(fileName) { }
        public override void WriteLine(string message)
        {
            base.WriteLine(DateTime.Now.ToString() + " " + message);
        }
    }
}
