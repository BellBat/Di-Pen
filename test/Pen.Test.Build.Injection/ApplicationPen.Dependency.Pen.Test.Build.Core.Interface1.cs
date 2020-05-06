namespace Pen.Test.Build.Injection
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        private Pen.Test.Build.Core.Interface1 _singlePen_Test_Build_Core_Interface1;
        private void LoadSinglePen_Test_Build_Core_Interface1()
        {
            Pen.Test.Build.Core.Interface1 result = new Pen.Test.Build.Core.Class1();



            // no decorators
            _singlePen_Test_Build_Core_Interface1 = result;		
        }

        private Pen.Test.Build.Core.Interface1 GetPen_Test_Build_Core_Interface1()
        {
            return _singlePen_Test_Build_Core_Interface1;
        }
    }
}