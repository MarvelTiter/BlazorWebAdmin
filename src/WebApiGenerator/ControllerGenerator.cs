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
                static (node, token) => true,
                static (c, t) => c);

            context.RegisterSourceOutput(list, static (context, source) =>
            {
                var methods = source.TargetNode.ChildNodes().Where(c => c is MethodDeclarationSyntax).Cast<MethodDeclarationSyntax>();
                List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();

                //var localField = FieldDeclaration(VariableDeclaration(IdentifierName(source.TargetSymbol.ToDisplayString())).AddVariables(VariableDeclarator(Identifier("proxyService")))).AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

                //var cBody = ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, IdentifierName("proxyService"), IdentifierName("service")));

                //var constructor = ConstructorDeclaration($"{WebApiGeneratorHepers.FormatClassName(source.TargetSymbol.MetadataName)}Controller")
                //    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                //    .AddParameterListParameters(
                //     Parameter(Identifier("service")).WithType(IdentifierName(source.TargetSymbol.ToDisplayString())))
                //.WithBody(Block(cBody));

                //members.Add(localField);
                //members.Add(constructor);

                foreach (var method in methods)
                {
                    //ArrowExpressionClause(InvocationExpression(
                    //        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("proxyService"), IdentifierName(method.Identifier.Text))
                    //        ).AddArgumentListArguments(method.ParameterList.Parameters.Select(p => Argument(IdentifierName(p.Identifier.Text))).ToArray()))).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                    var methodSyntax = MethodDeclaration(
                            method.ReturnType,
                            method.Identifier
                        ).AddParameterListParameters(method.ParameterList.Parameters.ToArray())
                        .AddAttributeLists(AttributeList(SingletonSeparatedList(
                            Attribute(IdentifierName("global::Microsoft.AspNetCore.Mvc.HttpGet"))
                            )))
                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                        .WithBody(method.Body);

                    members.Add(methodSyntax);
                }

                var unit = CompilationUnit()
                  .AddMembers(WebApiGeneratorHepers.CreateNamespaceDeclaration(source, out var usings)
                  .AddMembers(WebApiGeneratorHepers.CreateControllerClassDeclaration(source)
                        .AddMembers([.. members]))
                   ).AddUsings(usings).NormalizeWhitespace();
                //var text = unit.GetText(Encoding.UTF8).ToString();
                //context.AddSource($"{source.TargetSymbol.MetadataName}Controller.g.cs", unit.GetText(Encoding.UTF8));
            });
        }
    }
}
