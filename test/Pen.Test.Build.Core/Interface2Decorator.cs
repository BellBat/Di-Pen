using System;
using System.Collections.Generic;
using System.Text;

namespace Pen.Test.Build.Core
{
    public class Interface2Decorator : Interface2
    {
        private Interface2 _target;

        public Interface2Decorator(Interface2 target)
        {
            _target = target;
        }

        public void Do()
        {
            _target.Do();
        }
    }
}
