using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Pen.Declare;
using System.IO;

namespace Pen
{
    public static class Generate
    {
        private static string Output { get; set; }

        public static void Execute(string baseNamespace, string input, string output)
        {
            Output = output;

            FileDeleteIfExists(Path.Combine(output, "pen-generate.log"));
            FileDeleteIfExists(Path.Combine(output, "ApplicationPen.Dependency.cs"));

            foreach (var existingFile in Directory.GetFiles(output, "ApplicationPen.Dependency.*.cs"))
            {
                File.Delete(existingFile);
            }

            var classNamespace = baseNamespace;

            var inputDirectory = Path.GetDirectoryName(input);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                File.AppendAllText(Path.Combine(Output, "pen-generate.log"), $"Resolve:{args.Name}\n");
                string simpleName = args.Name.Split(',')[0] + ".dll";
                var assemblyPath = Path.Combine(inputDirectory, simpleName);
                if (File.Exists(assemblyPath))
                {
                    File.AppendAllText(Path.Combine(Output, "pen-generate.log"), $"Resolve:LoadFrom:{assemblyPath}\n");
                    return Assembly.LoadFrom(assemblyPath);
                }
                File.AppendAllText(Path.Combine(Output, "pen-generate.log"), $"Unresolved:{args.Name}\n");
                return null;
            };
            var assembly = Assembly.LoadFrom(input);
            var dependencies = ResolveDependencies(assembly).Values.ToArray();

            foreach (var dependency in dependencies)
            {
                File.WriteAllText(Path.Combine(output, $"ApplicationPen.Dependency.{dependency.Type.Namespace}.{dependency.Type.Name}.cs"), GetTemplateClassDependency(classNamespace, dependency));
            }

            File.WriteAllText(Path.Combine(output, "ApplicationPen.Dependency.cs"), GetTemplateDependency(classNamespace, dependencies));
        }

        private static Dictionary<Type, DependencyDefinition> ResolveDependencies(Assembly inAssembly)
        {
            var attributes = inAssembly.CustomAttributes;
            File.AppendAllText(Path.Combine(Output, "pen-generate.log"), "---\n");
            foreach(var attribute in attributes)
            {
                File.AppendAllText(Path.Combine(Output, "pen-generate.log"), attribute.AttributeType.AssemblyQualifiedName + "\n");
            }

            var dependencyAttributes = attributes
                .Where(item => item.AttributeType.AssemblyQualifiedName == typeof(DependencyAttribute).AssemblyQualifiedName)
                .Select(item => (IDependencyData)new DependencyData(item));
            File.AppendAllText(Path.Combine(Output, "pen-generate.log"), "---\n");
            foreach (var attribute in dependencyAttributes)
            {
                File.AppendAllText(Path.Combine(Output, "pen-generate.log"), $"{{\"type\":\"{attribute.Type}\"}}\n");
            }

            var decoratorAttributes = attributes
                .Where(item => item.AttributeType.AssemblyQualifiedName == typeof(DecoratorAttribute).AssemblyQualifiedName)
                .Select(item => (IDecoratorData)new DecoratorData(item));
            var decoratorAttributeLookup = decoratorAttributes.ToLookup(item => item.Dependency);

            var dependencies = new Dictionary<Type, DependencyDefinition>();
            
            // validate
            // ? implementation
            //     ? type interface the provider must be concrete and implement interface
            //     provider cannot be interface
            //     factory must be appropriately returning
            //     ? type is concrete the provider must be null or factory or subclass
            //     provider must be constructable

            
            foreach (var dependencyAttribute in dependencyAttributes)
            {
                var dependency = new DependencyDefinition() 
                { 
                    Declaration = dependencyAttribute,
                    Type = dependencyAttribute.Type,
                    Initializer = dependencyAttribute.Initializer,
                    Lifestyle = dependencyAttribute.Lifestyle
                };
                dependencies.Add(dependency.Type, dependency);
            }

            foreach (var value in dependencies.Values.ToList())
            {
                value.Provider = GetProvider(value.Declaration, dependencies);
            }

            foreach (var value in dependencies.Values)
            {
                {
                    if (value.Provider.Strategy == ProviderType.Implementation)
                    {
                        var constructor = value.Provider.Type.GetConstructors().Single();
                        var providerDependencies = new List<DependencyDefinition>();
                        foreach (var parameter in constructor.GetParameters())
                        {
                            providerDependencies.Add(dependencies[parameter.ParameterType]);
                        }
                        value.Provider.Dependencies = providerDependencies;
                    }
                    else if (value.Provider.Strategy == ProviderType.Factory)
                    {
                        foreach (var parameter in value.Provider.Method.GetParameters())
                        {
                            if (dependencies.TryGetValue(parameter.ParameterType, out var dependency))
                            {
                                value.Provider.Dependencies.Add(dependency);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                }

                if (decoratorAttributeLookup.Contains(value.Type))
                {
                    value.Decorators = decoratorAttributeLookup[value.Type]
                        .Select(item => new Decorator()
                        {
                            Type = item.Type
                        }).ToList();
                    foreach (var decorator in value.Decorators)
                    {
                        var constructor = decorator.Type.GetConstructors().Single();
                        var decoratorDependencies = new List<DependencyDefinition>();
                        bool targetFound = false;
                        foreach (var parameter in constructor.GetParameters())
                        {
                            if (parameter.ParameterType == value.Type)
                            {
                                if (!targetFound)
                                {
                                    targetFound = true;
                                    decoratorDependencies.Add(TargetDependency.Instance);
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            else
                            {
                                decoratorDependencies.Add(dependencies[parameter.ParameterType]);
                            }
                        }
                        decorator.Dependencies = decoratorDependencies;
                    }
                }
            }

            return dependencies;
        }

        private static ProviderDefinition GetProvider(IDependencyData from, Dictionary<Type, DependencyDefinition> dependencies)
        {
            var dependencyType = from.Type;
            var providerType = from.Provider;
            if (providerType == null)
            {
                return new ProviderDefinition()
                {
                    Strategy = ProviderType.Implementation,
                    Type = providerType
                };
            }
            else if (dependencyType.IsAssignableFrom(providerType))
            {
                return new ProviderDefinition()
                {
                    Strategy = ProviderType.Implementation,
                    Type = providerType
                };
            }
            else
            {
                var factoryMethods = providerType.GetMethods()
                    .Where(item => dependencyType.IsAssignableFrom(item.ReturnType));
                if (factoryMethods.Count() == 1)
                {
                    var factoryMethod = factoryMethods.Single();

                    var provider = new ProviderDefinition()
                    {
                        Strategy = ProviderType.Factory,
                        Type = providerType,
                        Method = factoryMethod,
                        Dependencies = new List<DependencyDefinition>()
                    };

                    if (!dependencies.ContainsKey(providerType))
                    {
                        dependencies.Add(
                            providerType,
                            new DependencyDefinition()
                            {
                                Declaration = null,
                                Lifestyle = Lifestyles.Transient,
                                Type = providerType,
                                Provider = new ProviderDefinition()
                                {
                                    Strategy = ProviderType.Implementation,
                                    Type = providerType
                                }
                            });
                    }

                    return provider;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private static string SafeName(Type ofType)
        {
            return ofType.FullName.Replace("_", "__").Replace(".", "_");
        }

        private static string _dependencyTemplateText =
@"namespace {{Namespace}}
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        public ApplicationPen()
        {
{{LoadSingles}}
        }

        public T Get<T>()
        {
            switch (typeof(T).FullName)
            {
{{ReturnInstances}}
                default:
                    throw new System.Exception();
            }
        }
    }
}";

        private static string GetTemplateDependency(string classNamespace, DependencyDefinition[] dependencies)
        {
            return _dependencyTemplateText
                .Replace("{{Namespace}}", classNamespace)
                .Replace("{{LoadSingles}}", BuildLoadSingles(dependencies))
                .Replace("{{ReturnInstances}}", BuildReturnInstances(dependencies));
        }

        private static string BuildLoadSingles(DependencyDefinition[] dependencies)
        {
            var builder = new StringBuilder();
            foreach (var dependency in dependencies.Where(item => item.Lifestyle == Lifestyles.Single))
            {
                builder.Append($"            LoadSingle");
                builder.Append(SafeName(dependency.Type));
                builder.AppendLine($"();");
            }
            return builder.ToString();
        }

        private static string BuildReturnInstances(DependencyDefinition[] dependencies)
        {
            var builder = new StringBuilder();
            foreach (var dependency in dependencies)
            {
                builder.Append("                case \"");
                builder.Append(dependency.Type.FullName);
                builder.AppendLine("\":");
                builder.Append("                    return (T)");
                if (!dependency.Type.IsInterface)
                {
                    builder.Append("(object)");
                }
                builder.Append("Get");
                builder.Append(SafeName(dependency.Type));
                builder.AppendLine("();");
            }
            return builder.ToString();
        }

        private static string _classDependencyTemplate =
@"namespace {{Namespace}}
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        private {{DependencyType}} Get{{SafeDependencyType}}()
        {
{{InstantiateProvider}}
{{Initializer}}
{{Decorators}}
            return result;
        }
    }
}";

        private static string _classDependencySingleTemplate =
@"namespace {{Namespace}}
{
    public partial class ApplicationPen : global::Pen.IPen
    {
        private {{DependencyType}} _single{{SafeDependencyType}};
        private void LoadSingle{{SafeDependencyType}}()
        {
{{InstantiateProvider}}
{{Initializer}}
{{Decorators}}
            _single{{SafeDependencyType}} = result;		
        }

        private {{DependencyType}} Get{{SafeDependencyType}}()
        {
            return _single{{SafeDependencyType}};
        }
    }
}";

        private static string GetTemplateClassDependency(string classNamespace, DependencyDefinition dependency)
        {
            var rawText = dependency.Lifestyle == Lifestyles.Single ? _classDependencySingleTemplate : _classDependencyTemplate;
            return rawText
                .Replace("{{Namespace}}", classNamespace)
                .Replace("{{DependencyType}}", dependency.Type.FullName)
                .Replace("{{SafeDependencyType}}", SafeName(dependency.Type))
                .Replace("{{InstantiateProvider}}", BuildInstantiation(dependency))
                .Replace("{{Initializer}}", BuildInitializer(dependency))
                .Replace("{{Decorators}}", BuildDecorators(dependency));
        }

        private static string BuildInstantiation(DependencyDefinition from)
        {
            var builder = new StringBuilder();
            switch (from.Provider.Strategy)
            {
                case ProviderType.Implementation:
                    {
                        builder.AppendIndent(3);
                        builder.Append(from.Type.FullName);
                        builder.Append(" result = new ");
                        builder.Append(from.Provider.Type.FullName);
                        builder.AppendMethodDependencies(from.Provider.Dependencies, 3);
                    }
                    break;
                case ProviderType.Factory:
                    {
                        builder.AppendGetDependency(from.Provider.Type, assignTo: "factory", indent: 3, end: true);
                        builder.AppendIndent(3);
                        builder.Append(from.Type.FullName);
                        builder.Append(" result = factory.");
                        builder.Append(from.Provider.Method.Name);
                        builder.AppendMethodDependencies(from.Provider.Dependencies, 3);
                    }
                    break;
            }

            builder.AppendLine();

            return builder.ToString();
        }

        private static string BuildInitializer(DependencyDefinition from)
        {
            // TODO: implement
            return string.Empty;
        }

        private static string BuildDecorators(DependencyDefinition from)
        {
            if (from?.Decorators?.Any() != true)
            {
                return "            // no decorators";
            }

            var builder = new StringBuilder();
            foreach (var decorator in from.Decorators)
            {
                builder.AppendIndent(3);
                builder.Append("result = new ");
                builder.Append(decorator.Type.FullName);
                builder.AppendMethodDependencies(decorator.Dependencies, 3);

                builder.AppendLine();
            }
            return builder.ToString();
        }


        private static void FileDeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    internal static class StringBuilderExtensions
    {
        private static string SafeName(Type ofType)
        {
            return ofType.FullName.Replace("_", "__").Replace(".", "_");
        }

        public static void AppendIndent(this StringBuilder target, int repeatCount = 1)
        {
            target.Append(' ' , 4 * repeatCount);
        }

        public static void AppendEndLine(this StringBuilder target)
        {
            target.AppendLine(";");
        }

        public static void AppendGetDependency(this StringBuilder target, Type ofType, string assignTo = null, int indent = 0, bool end = false)
        {
            if (indent > 0)
            {
                target.AppendIndent(indent);
            }

            if (!string.IsNullOrWhiteSpace(assignTo))
            {
                target.Append("var ");
                target.Append(assignTo);
                target.Append(" = ");
            }

            target.Append("Get");
            target.Append(SafeName(ofType));
            target.Append("()");
            if (end)
            {
                target.AppendEndLine();
            }
        }

        public static void AppendJoin<T>(this StringBuilder target, IEnumerable<T> values, Action<StringBuilder, T> appendValue, string delimiter = ",", bool breakAfterDelimiter = true)
        {
            bool first = true;
            foreach (var value in values)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    target.Append(delimiter);
                    if (breakAfterDelimiter)
                    {
                        target.AppendLine();
                    }
                }
                appendValue(target, value);
            }
        }

        public static void AppendMethodDependencies(this StringBuilder target, List<DependencyDefinition> dependencies, int methodIndent = 0)
        {
            if (dependencies?.Any() != true)
            {
                target.Append("()");
            }
            else if (dependencies.Count == 1)
            {
                target.Append("(");
                var dependency = dependencies.Single();
                if (dependency.IsTarget)
                {
                    target.Append("result");
                }
                else
                {
                    target.AppendGetDependency(dependency.Type);
                }
                target.Append(")");
            }
            else
            {
                target.AppendLine("(");
                target.AppendJoin(
                    dependencies,
                    (passthrough, dependency) =>
                    {
                        if (dependency.IsTarget)
                        {
                            passthrough.AppendIndent(methodIndent + 1);
                            passthrough.Append("result");
                        }
                        else
                        {
                            passthrough.AppendGetDependency(dependency.Type, indent: methodIndent + 1);
                        }
                    });
                target.Append(")");
            }
            target.AppendEndLine();
        }
    }


}
