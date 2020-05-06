using Pen;
using Pen.Declare;
using Pen.Test.Build.Core;

[assembly: Dependency(typeof(Interface1), typeof(Class1), Lifestyles.Single)]

[assembly: Dependency(typeof(Interface2), typeof(Class2))]
[assembly: Decorator(typeof(Interface2Decorator), typeof(Interface2))]
[assembly: Decorator(typeof(Interface2Decorator2), typeof(Interface2))]

[assembly: Dependency(typeof(Interface3), typeof(Class3Factory))]