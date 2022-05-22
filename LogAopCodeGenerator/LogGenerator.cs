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
            internal List<ClassDeclarationSyntax> SyntaxNodes { get; } = new List<ClassDeclarationSyntax>();
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    SyntaxNodes.Add(classDeclarationSyntax);
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

            var logInfoAttr = context.Compilation.GetTypeByMetadataName("LogAopCodeGenerator.LogInfoAttribute");
            var aspectableAttr = context.Compilation.GetTypeByMetadataName("LogAopCodeGenerator.AspectableAttribute");

            foreach (var node in receiver.SyntaxNodes)
            {
                AopImplClass(context, context.Compilation, node, logInfoAttr, aspectableAttr);
            }
        }

        private void AopImplClass(GeneratorExecutionContext context, Compilation compilation, ClassDeclarationSyntax classDeclaration, INamedTypeSymbol logInfo, INamedTypeSymbol aspectable)
        {
            //获得类的类型符号,如果无法获得，就跳出
            if (compilation.GetSemanticModel(classDeclaration.SyntaxTree).GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol) return;
            var interfaces = classSymbol.AllInterfaces;
            //类的特性
            ImmutableArray<AttributeData> allAttributes = classSymbol.GetAttributes()
                .Concat(classSymbol.AllInterfaces.SelectMany(i => i.GetAttributes()))
                .ToImmutableArray();

            if (allAttributes.Any(x => x.AttributeClass.Equals(aspectable, SymbolEqualityComparer.Default)))
            {
                BuildProxyClass(context, compilation, classDeclaration, classSymbol, interfaces, logInfo, allAttributes);
            }
        }

        private void BuildProxyClass(GeneratorExecutionContext context, Compilation compilation, ClassDeclarationSyntax classDeclaration, INamedTypeSymbol implType, ImmutableArray<INamedTypeSymbol> interfeceTypes, INamedTypeSymbol logInfo, ImmutableArray<AttributeData> attributes)
        {
            List<IMethodSymbol> allMethods = implType.GetMembers().Where(x => x.Kind == SymbolKind.Method)
                          .Cast<IMethodSymbol>()
                          .ToList();
            var np = classDeclaration.Parent as NamespaceDeclarationSyntax;
            var top = np.Parent as CompilationUnitSyntax;
            
            string proxyClassTemplate = $@"
{string.Join("",top?.Usings.Select(u=>u.ToFullString()))}
namespace {np?.Name}
{{
    public class {implType.Name}Proxy: {string.Join(",", interfeceTypes.Select(i => i.ToDisplayString()))}
    {{            
        {BuildConstructor(implType.Constructors.FirstOrDefault())}        
        {BuildMethodProxy(compilation, classDeclaration, allMethods)}        
    }}
}}
";
            context.AddSource($"{implType.Name}Proxy.cs", proxyClassTemplate);
        }
        private string BuildConstructor(IMethodSymbol ctor)
        {
            if (ctor == null) return @"
        // 没有构造函数
";
            StringBuilder builder = new StringBuilder();
            List<string> assigns = new List<string>();
            // 赋值与本地字段声明
            foreach (var p in ctor.Parameters)
            {
                builder.Append($@"
        private {p.Type.ToDisplayString()} {p.Name};
");
                assigns.Add($@"
            this.{p.Name} = {p.Name};
");
            }
            // 构造函数
            builder.Append($@"
        public {ctor.GetMethodName()}Proxy({string.Join(",", ctor.Parameters.Select(p => $"{p.Type.Name} {p.Name}"))})
        {{
            {string.Join("", assigns)}
        }}
");
            return builder.ToString();
        }
        private string BuildMethodProxy(Compilation compilation, ClassDeclarationSyntax classDeclaration, List<IMethodSymbol> methods)
        {
            StringBuilder builder = new StringBuilder();
            var cn = classDeclaration.GetCurrentNode(classDeclaration);
            var memberNodes = classDeclaration.Members.Select(m=>m as MethodDeclarationSyntax);
            foreach (var memberNode in memberNodes)
            {
                if (memberNode == null) continue;
                var method = compilation.GetSemanticModel(memberNode.SyntaxTree).GetDeclaredSymbol(memberNode) as IMethodSymbol;
                if (method.MethodKind == MethodKind.Constructor) continue;

                var parameters = method.Parameters;
                var plist = parameters.Select(p => $"{(p.IsParams ? "params " : "")}{p.Type.ToDisplayString()} {p.Name}").ToList();
                var asyncKeyWord = method.IsAsync ? " async " : " ";
                var returnString = method.ReturnsVoid ? "void" : method.ReturnType.ToDisplayString();
                builder.Append($@"
        private{asyncKeyWord}{returnString} internal{method.Name}({string.Join(",", plist)})        
{memberNode.Body.ToFullString()}
        
        public {(method.ReturnsVoid ? "void" : method.ReturnType.ToDisplayString())} {method.Name}({string.Join(",", plist)})
        {{
            Console.WriteLine(""from {method.Name} proxy"");
            {(method.ReturnsVoid ? "" : "return ")}internal{method.Name}({string.Join(",", parameters.Select(p => p.Name))});
        }}
");
            }
            return builder.ToString();
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
    }

}
