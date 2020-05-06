using System;
using System.Collections.Generic;
using System.Text;

namespace Pen.Test.Build.Core
{
    public class Class3Factory
    {
        private Interface1 _a; 

        public Class3Factory(Interface1 a)
        {
            _a = a;
        }

        public Class3 Hello(Interface2 b)
        {
            return new Class3();
        }
    }
}
