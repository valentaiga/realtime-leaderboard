using Microsoft.CodeAnalysis;

namespace Common.Filtering;

internal class FilterableObject(INamedTypeSymbol symbol, List<FilterableProperty> filterableProperties)
{
    public string Namespace => symbol.ContainingNamespace.ToDisplayString();
    public string ClassName => symbol.Name;
    public List<FilterableProperty> FilterableProperties { get; } = filterableProperties;
}

internal class FilterableProperty(IPropertySymbol symbol, byte[] filterableOperators)
{
    public byte[] FilterableOperators { get; } = filterableOperators;
    public string Name => symbol.Name;
    public string Type => symbol.Type.ToDisplayString();
    public bool IsParsable => IsBuiltInParsableType(symbol.Type) || IsIParsableType(symbol.Type);

    private static bool IsIParsableType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true, Name: "Nullable" } nullableNamedType)
        {
            // Unwrap Nullable<T> and check T
            var underlyingType = nullableNamedType.TypeArguments[0];
            return IsIParsableType(underlyingType);
        }
        
        if (typeSymbol is not INamedTypeSymbol namedType)
            return false;
        
        foreach (var inheritedTypeSymbol in namedType.AllInterfaces)
        {
            // Check for IParsable<T> where T is the type itself
            if (inheritedTypeSymbol is { IsGenericType: true, Name: "IParsable" } &&
                inheritedTypeSymbol.ContainingNamespace.ToDisplayString() == "System" &&
                inheritedTypeSymbol.TypeArguments.Length == 1 &&
                SymbolEqualityComparer.Default.Equals(inheritedTypeSymbol.TypeArguments[0], typeSymbol))
            {
                return true;
            }
        }

        return false;
    }
    
    private static bool IsBuiltInParsableType(ITypeSymbol typeSymbol)
    {
        var displayString = typeSymbol.ToDisplayString();

        return displayString is
            "string" or
            "int" or
            "long" or
            "short" or
            "byte" or
            "float" or
            "double" or
            "decimal" or
            "bool" or
            "System.DateTime" or
            "System.DateTimeOffset" or
            "System.DateOnly" or
            "System.TimeOnly" or
            "System.TimeSpan" or
            "System.Guid" or
            "System.Uri" or
            "System.Version" or
            "System.Numerics.BigInteger" or
            "System.Numerics.Complex" or
            "System.Half";
    }
}