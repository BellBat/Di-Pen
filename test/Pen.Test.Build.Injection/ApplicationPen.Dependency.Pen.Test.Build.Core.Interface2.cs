namespace Pen.Test.Build.Injection
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        private Pen.Test.Build.Core.Interface2 GetPen_Test_Build_Core_Interface2()
        {
            Pen.Test.Build.Core.Interface2 result = new Pen.Test.Build.Core.Class2(GetPen_Test_Build_Core_Interface1());



            result = new Pen.Test.Build.Core.Interface2Decorator(result);

            result = new Pen.Test.Build.Core.Interface2Decorator2(
                result,
                GetPen_Test_Build_Core_Interface1());


            return result;
        }
    }
}