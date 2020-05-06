using System;

namespace Pen.Build
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Generate.Execute(args[0], args[1], args[2]);
                /*Generate.Execute(
                    "Pen.Test.Build.Injection",
                    @"C:\code\Pen\Pen.Test.Build.Core\bin\Debug\netstandard2.0\Pen.Test.Build.Core.dll",
                    @"C:\code\Pen\Pen.Test.Build.Injection");*/

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Pen build failed. Message {ex.Message}");
                Console.Error.WriteLine(ex);
                return 1;
            }
        }
    }
}
