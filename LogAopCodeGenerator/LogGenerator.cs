using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LogAopCodeGenerator
{
    [Generator]
    public class LogGenerator : ISourceGenerator
    {
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            internal List<ClassDeclarationSyntax> ClassSyntaxNodes { get; } = new List<ClassDeclarationSyntax>();
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax @class)
                {
                    ClassSyntaxNodes.Add(@class);
                }
            }

        }
        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            var methodAopAttr = context.Compilation.GetTypeByMetadataName("LogAopCodeGenerator.AopMethodFlagAttribute");
            var aspectableAttr = context.Compilation.GetTypeByMetadataName("LogAopCodeGenerator.AspectableAttribute");

            foreach (var node in receiver.ClassSyntaxNodes)
            {
                AopImplClass(context, node, methodAopAttr, aspectableAttr);
            }
        }

        private void AopImplClass(GeneratorExecutionContext context, ClassDeclarationSyntax classDeclaration, INamedTypeSymbol methodAopAttr, INamedTypeSymbol aspectable)
        {
            // 获得类的类型描述符号,如果无法获得，就跳出
            var compilation = context.Compilation;
            if (compilation.GetSemanticModel(classDeclaration.SyntaxTree).GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol) return;

            // 所有特性
            ImmutableArray<AttributeData> allAttributes = classSymbol.GetAttributes()
                .Concat(classSymbol.AllInterfaces.SelectMany(i => i.GetAttributes()))
                .ToImmutableArray();

            if (allAttributes.Any(x => x.AttributeClass.Equals(aspectable, SymbolEqualityComparer.Default)))
            {
                BuildProxyClass(context, classDeclaration, classSymbol, methodAopAttr, aspectable, allAttributes);
            }
        }
        /// <summary>
        /// 创建Class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="compilation"></param>
        /// <param name="classDeclaration"></param>
        /// <param name="implType"></param>
        /// <param name="methodAopAttr"></param>
        /// <param name="attributes"></param>
        private void BuildProxyClass(GeneratorExecutionContext context, ClassDeclarationSyntax classDeclaration, INamedTypeSymbol implType, INamedTypeSymbol methodAopAttr, INamedTypeSymbol aspectable, ImmutableArray<AttributeData> attributes)
        {
            List<IMethodSymbol> allMethods = implType.GetMembers().Where(x => x.Kind == SymbolKind.Method)
                          .Cast<IMethodSymbol>()
                          .ToList();
            var usings = classDeclaration.BuildAllUsings();
            var npLabel = implType.ContainingNamespace.ToDisplayString();
            var interfaceNames = classDeclaration.GetInterfaceNames();
            var aopAttr = attributes.FirstOrDefault(a => a.AttributeClass.Equals(aspectable, SymbolEqualityComparer.Default));
            string ctors = BuildConstructor(implType, aopAttr, out var aopInstanceName);
            string body = BuildMethodProxy(context.Compilation, classDeclaration, implType, methodAopAttr, aopInstanceName);
            string proxyClassTemplate = BuildClassTemplate(usings, npLabel, implType.Name, interfaceNames, ctors, body);
            context.AddSource($"{implType.Name}Proxy.cs", proxyClassTemplate);
        }

        private string BuildClassTemplate(string[] usings, string np, string className, string[] interfacesString, params string[] bodys)
        {
            return $@"
{string.Join("", usings)}
using LogAopCodeGenerator;
namespace {np}
{{
    public class {className}Proxy: {string.Join(",", interfacesString)}
    {{            
         {string.Join("", bodys)}     
    }}
}}
";
        }

        /// <summary>
        /// 构建构造函数
        /// </summary>
        /// <param name="ctor"></param>
        /// <returns></returns>
        private string BuildConstructor(INamedTypeSymbol implType, AttributeData aopAttr, out string aopField)
        {
            var ctor = implType.Constructors.FirstOrDefault();
            StringBuilder builder = new StringBuilder();
            List<string> assigns = new List<string>();
            var handleType = aopAttr.NamedArguments.FirstOrDefault(x => x.Key == "AspectHandleType").Value.Value as ITypeSymbol;
            aopField = handleType.Name.ToLower();

            var fields = new List<FieldDefinition>();
            var aop = new FieldDefinition();
            aop.Name = aopField;
            aop.TypeName = handleType.ToDisplayString();
            fields.Add(aop);

            foreach (var p in ctor?.Parameters)
            {
                var pd = new FieldDefinition() { Name = p.Name, TypeName = p.Type.ToDisplayString() };
                fields.Add(pd);
            }
            return BuildConstructorTemplate(implType.Name, implType.Interfaces.FirstOrDefault()?.Name, fields.ToArray());
        }

        private string BuildConstructorTemplate(string className, string interfaceName, FieldDefinition[] fields)
        {
            return $@"
        {string.Join("", fields.Select(f => $"private {f.TypeName} {f.Name};\n"))}
        private BaseContext[] contexts;
        private {className} proxy;
        public {className}Proxy ({string.Join(",", fields.Select(f => $"{f.TypeName} {f.Name}"))})
        {{
            {string.Join("", fields.Select(f => $"this.{f.Name} = {f.Name};\n"))}
            contexts = InitContext.InitTypesContext(typeof({className}),typeof({interfaceName}));
            proxy = new {className}({string.Join(",", fields.Skip(1).Select(f => f.Name))});
        }}
";
        }

        /// <summary>
        /// 构建代理函数
        /// </summary>
        /// <param name="compilation"></param>
        /// <param name="classDeclaration"></param>
        /// <param name="methods"></param>
        /// <returns></returns>
        private string BuildMethodProxy(Compilation compilation, ClassDeclarationSyntax classDeclaration, INamedTypeSymbol implType, INamedTypeSymbol methodAopAttr, string aopInstanceField)
        {
            StringBuilder builder = new StringBuilder();
            var cn = classDeclaration.GetCurrentNode(classDeclaration);
            var memberNodes = classDeclaration.Members.Select(m => m as MethodDeclarationSyntax);
            foreach (var memberNode in memberNodes)
            {
                if (memberNode == null) continue;
                var method = compilation.GetSemanticModel(memberNode.SyntaxTree).GetDeclaredSymbol(memberNode) as IMethodSymbol;
                if (method.MethodKind == MethodKind.Constructor) continue;
                var parameters = method.Parameters;
                var md = new MemberDefinition();
                md.Name = method.Name;
                md.IsAsync = method.IsAsync;
                md.IsReturnVoid = method.ReturnsVoid;
                md.ReturnTypeString = method.ReturnType.ToDisplayString();
                md.Body = memberNode.Body.ToFullString();
                if (method.HasAttribute(methodAopAttr) || implType.HasAttribute(method, methodAopAttr))
                {
                    // 代理
                    builder.Append(BuildProxyMethodTemplate(md, parameters, aopInstanceField));
                }
                else
                {
                    // 不代理
                    builder.Append(BuildCommonMethodTemplate(md, parameters, aopInstanceField));
                }
            }
            return builder.ToString();
        }

        private string BuildProxyMethodTemplate(MemberDefinition member, ImmutableArray<IParameterSymbol> parameters, string aopInstanceField)
        {
            var plist = parameters.Select(p => $"{(p.IsParams ? "params " : "")}{p.Type.ToDisplayString()} {p.Name}").ToList();
            //private{member.AsyncKeyToken}{member.ReturnString} internal{member.Name}({string.Join(",", plist)})
            //{ member.Body}
            return $@"
        public{member.AsyncKeyToken}{member.ReturnString} {member.Name}({string.Join(",", plist)})
        {{
            var baseContext = contexts.First(x => x.ImplementMethod.Name == ""{member.Name}"");    
            var context = InitContext.BuildContext(baseContext);
            context.Exected = false;
            context.HasReturnValue = {(!member.IsReturnVoid).ToString().ToLower()};
            context.Parameters = new object[] {{ {string.Join(",", parameters.Select(p => p.Name))} }};
            {member.LocalVar}
            context.Proceed = {member.AsyncKeyToken}() => 
            {{
                context.Exected = true;
                {member.ReturnValueRecive}{member.AwaitKeyToken}proxy.{member.Name}({string.Join(",", parameters.Select(p => p.Name))}){member.InternalMethodResult};
                {member.ReturnValAsign}
                {member.TaskReturnLabel}
            }};
            {member.AwaitKeyToken}{aopInstanceField}.Invoke(context){member.WaitTask};
            {member.ReturnLabel}
        }}
";
        }

        private string BuildCommonMethodTemplate(MemberDefinition member, ImmutableArray<IParameterSymbol> parameters, string aopInstanceField)
        {
            var plist = parameters.Select(p => $"{(p.IsParams ? "params " : "")}{p.Type.ToDisplayString()} {p.Name}").ToList();
//        private{member.AsyncKeyToken}{member.ReturnString} internal{member.Name}({string.Join(",", plist)})
//{member.Body}
            return $@"
        public {member.ReturnString} {member.Name}({string.Join(",", plist)})
        {{            
             {(member.IsReturnVoid ? "" : "return ")}proxy.{member.Name}({string.Join(",", parameters.Select(p => p.Name))});                
        }}
";
        }
    }
    internal static class Ex
    {
        public static string GetMethodName(this IMethodSymbol symbol)
        {
            var parts = symbol.ToDisplayParts();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].ToString() == "(" && i > 0)
                {
                    return parts[i - 1].ToString();
                }
            }
            throw new Exception("找不到方法名");
        }

        public static string[] GetInterfaceNames(this ClassDeclarationSyntax @class)
        {
            var list = @class.BaseList?.Types;
            if (list == null) Array.Empty<string>();
            List<string> ret = new List<string>();
            foreach (var t in list)
            {
                if (t is SimpleBaseTypeSyntax @syntax)
                {
                    var ty = @syntax.Type as IdentifierNameSyntax;
                    if (ty != null)
                    {
                        ret.Add(ty.Identifier.ValueText);
                    }
                }
            }
            return ret.ToArray();
        }

        public static string[] BuildAllUsings(this ClassDeclarationSyntax @class, params InterfaceDeclarationSyntax[] interfaceDels)
        {
            Dictionary<string, int> nps = new Dictionary<string, int>();
            CompilationUnitSyntax top = @class.Parent?.Parent as CompilationUnitSyntax;
            AddNamespace(nps, top?.Usings.Select(n => new KeyValuePair<string, int>(n.ToFullString(), 0)));
            foreach (var item in interfaceDels)
            {
                CompilationUnitSyntax unit = item.Parent?.Parent as CompilationUnitSyntax;
                if (unit is null) continue;
                var i1 = unit.Usings.Select(n => new KeyValuePair<string, int>(n.ToFullString(), 0));
                AddNamespace(nps, i1);
            }
            return nps.Keys.ToArray();

            void AddNamespace(Dictionary<string, int> dic, IEnumerable<KeyValuePair<string, int>> values)
            {
                foreach (var item in values)
                {
                    dic[item.Key] = item.Value;
                }
            }
        }
        public static bool HasAttribute(this IMethodSymbol method, INamedTypeSymbol attr)
        {
            return method.GetAttributes().Any(x => x.AttributeClass.BaseType.Equals(attr, SymbolEqualityComparer.Default));
        }
        public static bool HasAttribute(this INamedTypeSymbol @class, IMethodSymbol method, INamedTypeSymbol attr)
        {
            var all = @class.AllInterfaces.SelectMany(i => i.GetMembers().Where(s => s.Kind == SymbolKind.Method && s.Name == method.Name).Select(s => s as IMethodSymbol));
            return all.SelectMany(a => a.GetAttributes()).Any(x => x.AttributeClass.BaseType.Equals(attr, SymbolEqualityComparer.Default));
        }
    }

}
