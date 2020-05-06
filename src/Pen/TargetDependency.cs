using System;
using System.Collections.Generic;
using System.Text;

namespace Pen
{
    public class TargetDependency : DependencyDefinition
    {
        private TargetDependency() { }

        public static TargetDependency Instance { get; } = new TargetDependency();
    }
}
