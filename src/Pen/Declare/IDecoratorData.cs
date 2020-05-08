using System;

namespace Pen.Declare
{
    public interface IDecoratorData
    {
        Type Dependency { get; set; }
        Type Type { get; }
    }
}