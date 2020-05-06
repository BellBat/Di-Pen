using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pen
{
    public class ProviderDefinition
    {
        public ProviderType Strategy { get; set; }

        public Type Type { get; set; }

        public MethodInfo Method { get; set; }

        public List<DependencyDefinition> Dependencies { get; set; }
    }
}
