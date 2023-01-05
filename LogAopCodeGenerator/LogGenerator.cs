using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                .Concat(classSymbol.BaseType?.GetAttributes())
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

            var aopAttr = attributes.FirstOrDefault(a => a.AttributeClass.Equals(aspectable, SymbolEqualityComparer.Default));
            string ctors = BuildConstructor(context.Compilation, implType, classDeclaration, aopAttr, out var aopInstanceName);
            string body = BuildMethodProxy(context.Compilation, classDeclaration, implType, methodAopAttr, aopInstanceName);
            string proxyClassTemplate = BuildClassTemplate(classDeclaration, implType, ctors, body);

            context.AddSource($"{implType.Name}Proxy.cs", proxyClassTemplate);
        }

        private string BuildClassTemplate(ClassDeclarationSyntax classDeclaration, INamedTypeSymbol implType, params string[] bodys)
        {
            // public 、 partial 之类的关键字
            var classModifiers = classDeclaration.Modifiers.Select(token => token.ValueText);
            return $@"
{string.Join("", classDeclaration.BuildAllUsings())}
using LogAopCodeGenerator;
namespace {implType.ContainingNamespace.ToDisplayString()}
{{
    {string.Join(",", classModifiers)} class {implType.Name}Proxy{classDeclaration.TypeParameterList?.ToFullString()}: {string.Join(",", classDeclaration.GetInterfaceNames())}
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
        private string BuildConstructor(Compilation compilation, INamedTypeSymbol implType, ClassDeclarationSyntax @class, AttributeData aopAttr, out string aopField)
        {
            var ctors = @class.Members.Where(m => m is ConstructorDeclarationSyntax).Cast<ConstructorDeclarationSyntax>().ToArray();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("private BaseContext[] contexts;");
            builder.AppendLine($"private {@class.Identifier.ValueText}{@class.TypeParameterList?.ToFullString()} _{@class.Identifier.ValueText}Proxy;");
            var handleType = aopAttr.NamedArguments.FirstOrDefault(x => x.Key == "AspectHandleType").Value.Value as ITypeSymbol;
            aopField = handleType.Name.ToLower();

            for (var i = 0; i < ctors.Length; i++)
            {
                var item = ctors[i];
                var fields = new List<FieldDefinition>();
                var aop = new FieldDefinition();
                aop.Name = aopField;
                aop.TypeName = handleType.ToDisplayString();
                //fields.Add(aop);
                foreach (var p in item.ParameterList.Parameters)//ctor?.Parameters
                {
                    var symbol = compilation.GetSemanticModel(p.SyntaxTree).GetDeclaredSymbol(p);
                    var fullTypeName = symbol.Type.ToDisplayString();
                    var name = symbol.Name;
                    if (fullTypeName == aop.TypeName)
                    {
                        aop.Name = name;
                        aopField = name;
                        aop.ProxyClassParameter = true;
                        aop.InBase = item.Initializer?.ArgumentList.Arguments.Any(ba => ba.ToFullString() == name) ?? false;
                        continue;
                    }
                    var pd = new FieldDefinition() { Name = name, TypeName = fullTypeName, ProxyClassParameter = true };
                    pd.InBase = item.Initializer?.ArgumentList.Arguments.Any(ba => ba.ToFullString() == name) ?? false;
                    fields.Add(pd);
                }
                fields.Insert(0, aop);
                var body = BuildConstructorTemplate(@class.Identifier.ValueText, @class.TypeParameterList?.ToFullString(), implType.Interfaces.FirstOrDefault()?.Name, fields.ToArray(), item.Initializer?.ToFullString());
                builder.Append(body);
            }
            if (ctors.Length == 0)
            {// 只有一个默认的构造函数
                var ctor = implType.Constructors.First();
                var fields = new List<FieldDefinition>();
                var aop = new FieldDefinition();
                aop.Name = aopField;
                aop.TypeName = handleType.ToDisplayString();
                fields.Add(aop);
                var gs = implType.TypeParameters.Length > 0 ? $"<{string.Join(",", implType.TypeParameters.Select(t => t.ToDisplayString()))}>" : "";
                var body = BuildConstructorTemplate(@class.Identifier.ValueText, gs, implType.Interfaces.FirstOrDefault()?.Name, fields.ToArray(), "");
                builder.Append(body);
            }
            return builder.ToString();
        }

        private string BuildConstructorTemplate(string className, string types, string interfaceName, FieldDefinition[] fields, string @base)
        {
            //private BaseContext[] contexts;
            //private {className} proxy;            
            return $@"
        {string.Join("", fields.Where(f => !f.InBase).Select(f => $"protected {f.TypeName} {f.Name};\n"))}
        public {className}Proxy ({string.Join(",", fields.Select(f => $"{f.TypeName} {f.Name}"))}) {@base}
        {{
            {string.Join("", fields.Where(f => !f.InBase).Select(f => $"this.{f.Name} = {f.Name};\n"))}
            contexts = InitContext.InitTypesContext(typeof({className}{types}),typeof({interfaceName}{types}));
            _{className}Proxy = new {className}{types}({string.Join(",", fields.Where(f => f.ProxyClassParameter).Select(f => f.Name))});
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
            var memberNodes = classDeclaration.Members.Select(m => m as MethodDeclarationSyntax);
            var proxyInstanceName = $"_{classDeclaration.Identifier.ValueText}Proxy";
            foreach (var memberNode in memberNodes)
            {
                if (memberNode == null) continue;
                var method = compilation.GetSemanticModel(memberNode.SyntaxTree).GetDeclaredSymbol(memberNode);
                if (method.MethodKind == MethodKind.Constructor) continue;
                var parameters = method.Parameters;
                var md = new MemberDefinition();
                md.Name = method.Name;
                md.IsAsync = method.IsAsync;
                md.IsReturnVoid = method.ReturnsVoid;
                md.IsOverride = method.IsOverride;
                md.IsVirtual = method.IsVirtual;
                md.ReturnTypeString = method.ReturnType.ToDisplayString();
                md.Body = memberNode.Body.ToFullString();
                if (method.HasAttribute(methodAopAttr) || implType.HasAttribute(method, methodAopAttr))
                {
                    // 代理
                    builder.Append(BuildProxyMethodTemplate(classDeclaration.Identifier.ValueText, proxyInstanceName, md, parameters, aopInstanceField));
                }
                else
                {
                    // 不代理
                    builder.Append(BuildCommonMethodTemplate(proxyInstanceName, md, parameters, aopInstanceField));
                }
            }
            return builder.ToString();
        }

        private string BuildProxyMethodTemplate(string unitPrefix, string proxyInstance, MemberDefinition member, ImmutableArray<IParameterSymbol> parameters, string aopInstanceField)
        {
            var plist = parameters.Select(p => $"{(p.IsParams ? "params " : "")}{p.Type.ToDisplayString()} {p.Name}").ToList();
            //private{member.AsyncKeyToken}{member.ReturnString} internal{member.Name}({string.Join(",", plist)})
            //{ member.Body}
            return $@"
        public{member.OverrideToken}{member.VirtualToken}{member.AsyncKeyToken}{member.ReturnString} {member.Name}({string.Join(",", plist)})
        {{
            var baseContext = contexts.First(x => x.ImplementMethod.Name == ""{member.Name}"");    
            var _{unitPrefix}context = InitContext.BuildContext(baseContext);
            _{unitPrefix}context.Exected = false;
            _{unitPrefix}context.HasReturnValue = {(!member.IsReturnVoid).ToString().ToLower()};
            _{unitPrefix}context.Parameters = new object[] {{ {string.Join(",", parameters.Select(p => p.Name))} }};
            {member.LocalVar}
            _{unitPrefix}context.Proceed = {member.AsyncKeyToken}() => 
            {{
                _{unitPrefix}context.Exected = true;
                {member.ReturnValueRecive}{member.AwaitKeyToken}{proxyInstance}.{member.Name}({string.Join(",", parameters.Select(p => p.Name))}){member.InternalMethodResult};
                {member.ReturnValAsign($"_{unitPrefix}context")}
                {member.TaskReturnLabel}
            }};
            {member.AwaitKeyToken}{aopInstanceField}.Invoke(_{unitPrefix}context){member.WaitTask};
            {member.ReturnLabel}
        }}
";
        }

        private string BuildCommonMethodTemplate(string proxyInstance, MemberDefinition member, ImmutableArray<IParameterSymbol> parameters, string aopInstanceField)
        {
            var plist = parameters.Select(p => $"{(p.IsParams ? "params " : "")}{p.Type.ToDisplayString()} {p.Name}").ToList();
            //        private{member.AsyncKeyToken}{member.ReturnString} internal{member.Name}({string.Join(",", plist)})
            //{member.Body}
            return $@"
        public {member.ReturnString} {member.Name}({string.Join(",", plist)})
        {{            
             {(member.IsReturnVoid ? "" : "return ")}{proxyInstance}.{member.Name}({string.Join(",", parameters.Select(p => p.Name))});                
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
                    if (syntax.Type is IdentifierNameSyntax ty)
                    {
                        if (ty != null)
                        {

                            ret.Add(ty.Identifier.ValueText);
                        }
                    }
                    else if (syntax.Type is GenericNameSyntax g)
                    {
                        if (g != null)
                        {
                            ret.Add($"{g.Identifier.ValueText}{g.TypeArgumentList}");
                        }
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
