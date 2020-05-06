namespace Pen.Test.Build.Core
{
    public class Interface2Decorator2 : Interface2
    {
        private Interface2 _target;
        private Interface1 _a;

        public Interface2Decorator2(Interface2 target, Interface1 a)
        {
            _target = target;
            _a = a;
        }

        public void Do()
        {
            _target.Do();
        }
    }
}
