using System;
using System.Collections.Generic;
using System.Text;

namespace Sabatex.Extensions.Text
{
    public class EncodingProvider1251 : EncodingProvider
    {
        public override Encoding GetEncoding(int codepage)
        {
            if (codepage == 1251)
                return new Encoding1251();
            throw new NotImplementedException();
        }

        public override Encoding GetEncoding(string name)
        {
            if (name.ToLower() == "windows-1251")
                return new Encoding1251();

            throw new NotImplementedException();
        }
    }
}
