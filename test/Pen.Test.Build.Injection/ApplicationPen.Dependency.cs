namespace Pen.Test.Build.Injection
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        public ApplicationPen()
        {
            LoadSinglePen_Test_Build_Core_Interface1();

        }

        public T Get<T>()
        {
            switch (typeof(T).FullName)
            {
                case "Pen.Test.Build.Core.Interface1":
                    return (T)GetPen_Test_Build_Core_Interface1();
                case "Pen.Test.Build.Core.Interface2":
                    return (T)GetPen_Test_Build_Core_Interface2();
                case "Pen.Test.Build.Core.Interface3":
                    return (T)GetPen_Test_Build_Core_Interface3();
                case "Pen.Test.Build.Core.Class3Factory":
                    return (T)(object)GetPen_Test_Build_Core_Class3Factory();

                default:
                    throw new System.Exception();
            }
        }
    }
}