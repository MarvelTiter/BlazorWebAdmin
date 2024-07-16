using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;
namespace WebApiGenerator.Models
{
    public record TypeInfo(string QualifiedName, TypeKind Kind, bool IsRecord)
    {

    }
    public record HierarchyInfo(string FilenameHint, string MetadataName, string Namespace, ImmutableArray<TypeInfo> Hierarchy)
    {
        public static HierarchyInfo From(INamedTypeSymbol typeSymbol)
        {
            List<TypeInfo> hierarchy = new();

            for (INamedTypeSymbol? parent = typeSymbol;
                 parent is not null;
                 parent = parent.ContainingType)
            {
                hierarchy.Add(new TypeInfo(
                    parent.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    parent.TypeKind,
                    parent.IsRecord));
            }

            return new(
                $"{typeSymbol.ContainingNamespace.ToDisplayString()}.{typeSymbol.Name}",
                typeSymbol.MetadataName,
                typeSymbol.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: NameAndContainingTypesAndNamespaces)),
                [.. hierarchy]);
        }
    }
}
