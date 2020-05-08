using System;

namespace Pen.Declare
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DecoratorAttribute : Attribute, IDecoratorData
    {
        public DecoratorAttribute(Type type, Type dependency)
        {
            Type = type;
            Dependency = dependency;
        }

        public Type Type { get; private set; }

        public Type Dependency { get; set; }
    }
}
