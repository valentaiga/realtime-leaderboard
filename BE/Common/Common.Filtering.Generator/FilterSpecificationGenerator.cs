using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Common.Filtering;

/// <summary>
/// FilterSpecification generator for all classes with FilterSpecificationAttribute. <br/>
/// Requires FilterCriteriaAttribute for properties which can be searched by.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class FilterSpecificationGenerator : IIncrementalGenerator
{
    private const string FilterSpecificationAttributeFullName = "Common.Filtering.FilterSpecificationAttribute";
    private const string FilterCriteriaAttributeFullName = "Common.Filtering.FilterCriteriaAttribute";

    /// <summary> Entrypoint for roslyn analyzer. </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource("FilterCriteriaAttribute.g.cs", SourceText.From(SourceGenerationHelper.FilterCriteriaAttribute, Encoding.UTF8));
            ctx.AddSource("FilterSpecificationAttribute.g.cs", SourceText.From(SourceGenerationHelper.FilterSpecificationAttribute, Encoding.UTF8));
        });

        var objectsToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FilterSpecificationAttributeFullName,
                predicate: static (node, _) => node is  ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: GetSemanticTargetForGeneration)
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);
        
        context.RegisterSourceOutput(objectsToGenerate, GenerateCode);
    }

    private static FilterableObject? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;

        var hasAttribute = symbol.GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == FilterSpecificationAttributeFullName);
        if (!hasAttribute)
            return null;

        var result = new FilterableObject(symbol, []);

        foreach (var prop in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            ct.ThrowIfCancellationRequested();
            var attr = prop.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == FilterCriteriaAttributeFullName);
        
            if (attr is null || attr.ConstructorArguments.Length == 0)
                continue;
        
            var operators = attr.ConstructorArguments[0].Values
                .OfType<TypedConstant>()
                .Select(tc => (byte)tc.Value!)
                .ToArray();
        
            result.FilterableProperties.Add(new FilterableProperty(prop, operators));
        }

        return result;
    }

    private static void GenerateCode(SourceProductionContext context, FilterableObject target)
    {
        var code = SourceGenerationHelper.GenerateFilterSpecificationV2(target);
        context.AddSource($"{target.ClassName}FilterSpecification.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}