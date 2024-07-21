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
            var list = context.SyntaxProvider.ForAttributeWithMetadataName(
                WebApiGeneratorHepers.WebControllerAttributeFullName,
                static (node, token) => node is ClassDeclarationSyntax,
                static (c, t) => c);

            context.RegisterSourceOutput(list, static (context, source) =>
            {
                var classSymbol = source.TargetSymbol as INamedTypeSymbol;
                var interfaceSymbol = classSymbol!.Interfaces.FirstOrDefault(c => c.GetAttribute<Attributes.WebControllerAttribute>(out _));
                if (interfaceSymbol == null)
                {
                    return;
                }
                if (!interfaceSymbol.GetAttribute<Attributes.WebControllerAttribute>(out var attributeData))
                {
                    context.ReportDiagnostic(DiagnosticDefinitions.WAG00001(source.TargetNode.GetLocation()));
                    return;
                }
                var methods = interfaceSymbol.GetAllMethods();
                List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

                var localField = FieldDeclaration(VariableDeclaration(IdentifierName(interfaceSymbol.ToDisplayString())).AddVariables(VariableDeclarator(Identifier("proxyService")))).AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

                var cBody = ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("proxyService"), IdentifierName("service")));

                var constructor = ConstructorDeclaration($"{WebApiGeneratorHepers.FormatClassName(interfaceSymbol.MetadataName)}Controller")
                    .AddAttributeLists(
                     AttributeList(SingletonSeparatedList(
                     Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).Assembly.GetName().Version.ToString())))
                         )
                     )))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(Parameter(Identifier("service")).WithType(IdentifierName(interfaceSymbol.ToDisplayString())))
                .WithBody(Block(cBody));

                members.Add(localField);
                members.Add(constructor);

                foreach (var methodSymbol in methods)
                {
                    string httpMethod;
                    if (methodSymbol.GetAttribute<Attributes.WebMethodAttribute>(out var a))
                    {
                        httpMethod = a.GetAttributeValue<HttpMethod>(nameof(Attributes.WebMethodAttribute.Method)) switch
                        {
                            HttpMethod.Get => "HttpGet",
                            HttpMethod.Post => "HttpPost",
                            HttpMethod.Put => "HttpPut",
                            HttpMethod.Delete => "HttpDelete",
                            _ => "HttpGet"
                        };
                    }
                    else
                    {
                        httpMethod = TryGetHttpMethodFromMethodName(methodSymbol.Name);
                    }
                    var methodSyntax = MethodDeclaration(
                                    IdentifierName(methodSymbol.ReturnType.ToDisplayString()),
                                    Identifier(methodSymbol.Name)
                                ).AddParameterListParameters([.. methodSymbol.Parameters.Select(p => Parameter(
                                    [],[],IdentifierName(p.Type.ToDisplayString()),Identifier(p.Name),null))])
                                .AddAttributeLists(AttributeList(SingletonSeparatedList(
                                    Attribute(IdentifierName($"global::Microsoft.AspNetCore.Mvc.{httpMethod}"))
                                    .AddArgumentListArguments(AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(a.GetAttributeValue("Route")?.ToString() ?? methodSymbol.Name.Replace("Async", "")))))
                                    ))
                                , AttributeList(SingletonSeparatedList(
                                     Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                                        .AddArgumentListArguments(
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).FullName))),
                                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ControllerGenerator).Assembly.GetName().Version.ToString())))
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
                                
                var unit = CompilationUnit()
                  .AddMembers(WebApiGeneratorHepers.CreateNamespaceDeclaration(source, out var usings)
                  .AddMembers(WebApiGeneratorHepers.CreateControllerClassDeclaration(source)
                        .AddMembers([.. members]))
                   ).AddUsings(usings).NormalizeWhitespace();
                var text = unit.GetText(Encoding.UTF8).ToString();
                context.AddSource($"{interfaceSymbol.MetadataName}Controller.g.cs", unit.GetText(Encoding.UTF8));
            });
        }

        private static string TryGetHttpMethodFromMethodName(string name)
        {
            if (name.StartsWith("create", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("add", StringComparison.OrdinalIgnoreCase))
            {
                return "HttpPost";
            }
            else if (name.StartsWith("get", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("find", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("query", StringComparison.OrdinalIgnoreCase))
            {
                return "HttpGet";
            }
            else if (name.StartsWith("update", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("put", StringComparison.OrdinalIgnoreCase))
            {
                return "HttpPut";
            }
            else if (name.StartsWith("delete", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("remove", StringComparison.OrdinalIgnoreCase))
            {
                return "HttpDelete";
            }
            else
            {
                return "HttpGet";
            }
        }
    }
}
