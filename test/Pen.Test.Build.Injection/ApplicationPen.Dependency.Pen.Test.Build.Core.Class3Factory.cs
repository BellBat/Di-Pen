namespace Pen.Test.Build.Injection
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        private Pen.Test.Build.Core.Class3Factory GetPen_Test_Build_Core_Class3Factory()
        {
            Pen.Test.Build.Core.Class3Factory result = new Pen.Test.Build.Core.Class3Factory(GetPen_Test_Build_Core_Interface1());



            // no decorators
            return result;
        }
    }
}