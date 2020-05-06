namespace Pen.Test.Build.Core
{
    public class Class2 : Interface2
    {
        public Class2(Interface1 a)
        {
            A = a;
        }

        public Interface1 A { get; set; }

        public void Do() { }
    }
}
