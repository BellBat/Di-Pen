using System;
using System.Collections.Generic;
using System.Text;

namespace Pen
{
    public class Decorator
    {
        public Type Type { get; set; }

        public List<DependencyDefinition> Dependencies { get; set; }
    }
}
