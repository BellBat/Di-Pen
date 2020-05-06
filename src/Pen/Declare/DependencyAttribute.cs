using System;

namespace Pen.Declare
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
        public DependencyAttribute(Type type, Type provider = null, Lifestyles lifestyle = Lifestyles.Transient, Type initializer = null)
        {
            Type = type;
            Provider = provider;
            Lifestyle = lifestyle;
            Initializer = initializer;
        }

        public Type Type { get; private set; }

        public Type Provider { get; private set; }

        public Lifestyles Lifestyle { get; private set; }

        public Type Initializer { get; private set; }
    }
}
