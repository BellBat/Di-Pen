using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pen.Declare
{
    public class DecoratorData : IDecoratorData
    {
        public DecoratorData() { }

        public DecoratorData(CustomAttributeData customAttributeData) 
        {
            Type = (Type)customAttributeData.ConstructorArguments[0].Value;
            Dependency = (Type)customAttributeData.ConstructorArguments[1].Value;
        }

        public Type Type { get; private set; }

        public Type Dependency { get; set; }
    }
}
