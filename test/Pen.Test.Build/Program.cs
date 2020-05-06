using Pen.Test.Build.Core;
using Pen.Test.Build.Injection;

namespace Pen.Test.Build
{
    class Program
    {
        static void Main(string[] args)
        {
            IPen pen = new ApplicationPen();
            var i2 = pen.Get<Interface2>();
            i2 = null;
        }
    }
}
