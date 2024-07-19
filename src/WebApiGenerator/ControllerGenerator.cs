using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebApiGenerator.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace WebApiGenerator
{
    [Generator]
    public class ControllerGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var list = context.SyntaxProvider.ForAttributeWithMetadataName(WebApiGeneratorHepers.WebControllerAttributeFullName,
                static (node, token) => node is ClassDeclarationSyntax,
                static (c, t) => c);

            context.RegisterSourceOutput(list, static (context, source) =>
            {
                var classSymbol = source.TargetSymbol as INamedTypeSymbol;
                //if (!Debugger.IsAttached)
                //{
                //    Debugger.Launch();
                //}
                if (!classSymbol!.AllInterfaces.Any(a => a.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == WebApiGeneratorHepers.WebControllerAttributeFullName)))
                {
        //            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
        //                id: "IG00001",
        //title: "接口未标注[WebControllerAttribute]",
        //messageFormat: "接口未标注[WebControllerAttribute]",
        //category: typeof(ControllerGenerator).FullName,
        //defaultSeverity: DiagnosticSeverity.Error,
        //isEnabledByDefault: true,
        //description: "The source generator features from the MVVM Toolkit require consuming projects to set the C# language version to at least C# 8.0. Make sure to add <LangVersion>8.0</LangVersion> (or above) to your .csproj file."), source.TargetNode.GetLocation()));
                    return;
                }
                var methods = WebApiGeneratorHepers.GetAllMethods(classSymbol);
                List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

                var ctorSyntax = (ConstructorDeclarationSyntax)source.TargetNode.ChildNodes().First(c => c is ConstructorDeclarationSyntax);


                var localField = FieldDeclaration(VariableDeclaration(IdentifierName(source.TargetSymbol.ToDisplayString())).AddVariables(VariableDeclarator(Identifier("proxyService")))).AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

                var cBody = ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("proxyService"), ObjectCreationExpression(IdentifierName(source.TargetSymbol.ToDisplayString())).AddArgumentListArguments([.. ctorSyntax.ParameterList.Parameters.Select(p => Argument(IdentifierName(p.Identifier.Text)))])));

                var constructor = ConstructorDeclaration($"{WebApiGeneratorHepers.FormatClassName(source.TargetSymbol.MetadataName)}Controller")
                    .AddAttributeLists(
                     AttributeList(SingletonSeparatedList(
                     Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).Assembly.GetName().Version.ToString())))
                         )
                     )))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters([.. ctorSyntax.ParameterList.Parameters])
                .WithBody(Block(cBody));

                members.Add(localField);
                members.Add(constructor);

                foreach (var methodSymbol in methods)
                {
                    var a = methodSymbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == WebApiGeneratorHepers.WebMethodAttributeFullName);
                    var methodSyntax = MethodDeclaration(
                                    IdentifierName(methodSymbol.ReturnType.ToDisplayString()),
                                    Identifier(methodSymbol.Name)
                                ).AddParameterListParameters([.. methodSymbol.Parameters.Select(p => Parameter(
                                    [],[],IdentifierName(p.Type.ToDisplayString()),Identifier(p.Name),null))])
                                .AddAttributeLists(AttributeList(SingletonSeparatedList(
                                    Attribute(IdentifierName("global::Microsoft.AspNetCore.Mvc.HttpGet"))
                                    .AddArgumentListArguments(AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(a.GetAttributeValue("Route")?.ToString() ?? methodSymbol.Name.Replace("Async", "")))))
                                    ))
                                , AttributeList(SingletonSeparatedList(
                                     Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                                        .AddArgumentListArguments(
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof           (ControllerGenerator).FullName))),
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof              (ControllerGenerator).Assembly.GetName().Version.ToString())))
                                         )
                                     ))
                                )
                                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("proxyService"), IdentifierName(methodSymbol.Name))
                                ).AddArgumentListArguments([.. methodSymbol.Parameters.Select(p => Argument(IdentifierName(p.Name)))]))).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                    //var methodText = methodSyntax.GetText(Encoding.UTF8).ToString();

                    members.Add(methodSyntax);
                }

                //var tree = source.SemanticModel.Compilation.GetSemanticModel(source.TargetNode.SyntaxTree);

                //var syntaxc = source.TargetSymbol.DeclaringSyntaxReferences;
                //var cn = (INamedTypeSymbol)source.TargetSymbol;
                //var ems = cn.GetMembers();
                //var ms = cn.BaseType?.GetMembers();
                ////var f = ms.Value.First() as IFieldSymbol;
                ////var syntax = f.DeclaringSyntaxReferences.First().GetSyntax();
                //foreach (var e in ms ?? [])
                //{
                //    var syntax = e.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
                //}

                var unit = CompilationUnit()
                  .AddMembers(WebApiGeneratorHepers.CreateNamespaceDeclaration(source, out var usings)
                  .AddMembers(WebApiGeneratorHepers.CreateControllerClassDeclaration(source)
                        .AddMembers([.. members]))
                   ).AddUsings(usings).NormalizeWhitespace();
                //var text = unit.GetText(Encoding.UTF8).ToString();
                context.AddSource($"{source.TargetSymbol.MetadataName}Controller.g.cs", unit.GetText(Encoding.UTF8));
            });
        }
    }
}
