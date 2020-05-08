using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pen.Declare
{
    public class DependencyData : IDependencyData
    {
        public DependencyData() 
        { 
        }

        public DependencyData(CustomAttributeData customAttributeData)
        {
            Type = (Type)customAttributeData.ConstructorArguments[0].Value;
            Provider = (Type)customAttributeData.ConstructorArguments[1].Value;
            Lifestyle = (Lifestyles)customAttributeData.ConstructorArguments[2].Value;
            Initializer = (Type)customAttributeData.ConstructorArguments[3].Value;
        }

        public Type Type { get; private set; }

        public Type Provider { get; private set; }

        public Lifestyles Lifestyle { get; private set; }

        public Type Initializer { get; private set; }
    }
}
