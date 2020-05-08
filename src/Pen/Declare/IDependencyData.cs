using System;

namespace Pen.Declare
{
    public interface IDependencyData
    {
        Type Initializer { get; }
        Lifestyles Lifestyle { get; }
        Type Provider { get; }
        Type Type { get; }
    }
}