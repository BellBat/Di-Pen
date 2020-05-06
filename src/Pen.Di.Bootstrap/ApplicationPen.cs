using System;

namespace Pen.Di.Bootstrap
{
    public class ApplicationPen : IPen
    {
        public T Get<T>()
        {
            throw new NotImplementedException("This is the bootstrap assembly - codegen needs to replace it still.");
        }
    }
}
