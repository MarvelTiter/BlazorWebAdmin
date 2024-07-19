using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace WebApiGenerator
{
    public static class WebApiGeneratorHepers
    {
        public const string WebControllerAttributeFullName = "WebApiGenerator.Attributes.WebControllerAttribute";
        public const string WebMethodAttributeFullName = "WebApiGenerator.Attributes.WebMethodAttribute";

        public static string FormatClassName(string className)
        {
            if (className.IndexOf('`') > -1)
            {
                return className.Substring(0, className.IndexOf('`'));
            }
            return className;
        }

        public static object? GetAttributeValue(this AttributeData? a, string key)
        {
            if (a == null) return null;
            var c = a.NamedArguments.FirstOrDefault(t => t.Key == key);
            return c.Value.Value;
        }

        public static T? GetAttributeValue<T>(this AttributeData? a, string key)
        {
            var t = GetAttributeValue(a, key);
            if (t == null) return default;
            return (T)t;
        }

        public static bool GetAttribute<T>(this ISymbol symbol, out AttributeData? data)
        {
            data = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == typeof(T).FullName);
            return data != null;
        }

        public static IEnumerable<IMethodSymbol> GetAllMethods(INamedTypeSymbol? symbol)
        {
            for (var b = symbol; b != null; b = b.BaseType)
            {
                if (b.Name == nameof(Object))
                    yield break;
                foreach (var item in b.GetMembers().Where(m => m is IMethodSymbol).Cast<IMethodSymbol>())
                {
                    if (item.MethodKind == MethodKind.Constructor)
                    {
                        continue;
                    }

                    yield return b.IsGenericType ? item.ConstructedFrom : item;
                }
            }
        }

        public static ClassDeclarationSyntax CreateControllerClassDeclaration(GeneratorAttributeSyntaxContext source)
        {
            var cd = ClassDeclaration($"{FormatClassName(source.TargetSymbol.MetadataName)}Controller");
            var classNode = (ClassDeclarationSyntax)source.TargetNode;
            if (classNode is { TypeParameterList: var list and not null })
            {
                cd = cd.AddTypeParameterListParameters([.. list.Parameters]).AddConstraintClauses([.. classNode.ConstraintClauses]);
            }
            var controllerAttribute = source.TargetSymbol.GetAttributes().First(a => a.AttributeClass?.ToDisplayString() == WebControllerAttributeFullName);
            var route = controllerAttribute.GetAttributeValue(nameof(WebApiGenerator.Attributes.WebControllerAttribute.Route)) ?? "[controller]";
            cd = cd.AddBaseListTypes(SimpleBaseType(IdentifierName("global::Microsoft.AspNetCore.Mvc.ControllerBase")))
             .AddAttributeLists(AttributeList(SingletonSeparatedList(
                 Attribute(IdentifierName("global::Microsoft.AspNetCore.Mvc.ApiController"))
                 )),
                 AttributeList(SingletonSeparatedList(
                 Attribute(IdentifierName("global::Microsoft.AspNetCore.Mvc.Route")).AddArgumentListArguments(
                     AttributeArgument(LiteralExpression(
                         SyntaxKind.StringLiteralExpression,
                         Literal($"api/{route}")
                         ))
                     )
                 ))
                 //, AttributeList(SingletonSeparatedList(
                 //    Attribute((IdentifierName("global::Microsoft.AspNetCore.Authorization.Authorize")))
                 //    ))
                 , AttributeList(SingletonSeparatedList(
                     Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).Assembly.GetName().Version.ToString())))
                         )
                     ))
                 )
             .AddModifiers(Token(TriviaList(Comment("/// <inheritdoc/>")), SyntaxKind.PublicKeyword, TriviaList()))
             .AddMembers(classNode.ChildNodes().Where(n => n is FieldDeclarationSyntax).Cast<FieldDeclarationSyntax>().ToArray());

            return cd;
        }

        public static CompilationUnitSyntax CreateCompilationUnit(this GeneratorAttributeSyntaxContext source)
        {
            //CompilationUnit()
            //    .AddMembers(NamespaceDeclaration())
            return CompilationUnit();
        }

        public static NamespaceDeclarationSyntax CreateNamespaceDeclaration(GeneratorAttributeSyntaxContext source, out UsingDirectiveSyntax[] usings)
        {
            var np = NamespaceDeclaration(IdentifierName(source.TargetSymbol.ContainingNamespace.ToDisplayString()))
                .WithLeadingTrivia(Comment("// <auto-generated/>"), Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)));
            //source.TargetSymbol.con
            if (source.TargetNode is
                {

                    Parent: NamespaceDeclarationSyntax
                    {
                        Usings: var nu,
                        Parent: CompilationUnitSyntax
                        {
                            Usings: var cnu
                        }
                    }
                }
                )
            {
                usings = [.. nu, .. cnu];
            }
            else
            {
                usings = [];
            }

            return np;
        }
    }



}
