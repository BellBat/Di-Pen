namespace Pen.Test.Build.Injection
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        private Pen.Test.Build.Core.Interface3 GetPen_Test_Build_Core_Interface3()
        {
            var factory = GetPen_Test_Build_Core_Class3Factory();
            Pen.Test.Build.Core.Interface3 result = factory.Hello(GetPen_Test_Build_Core_Interface2());



            // no decorators
            return result;
        }
    }
}