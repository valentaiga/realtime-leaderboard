using System.Text;

namespace Common.Filtering;

internal class SourceGenerationHelper
{
    private const string Tab = "    ";
    private const string Tab2 = $"{Tab}{Tab}";
    private const string Tab3 = $"{Tab}{Tab}{Tab}";
    private const string Tab4 = $"{Tab}{Tab}{Tab}{Tab}";
    private const string Tab5 = $"{Tab}{Tab}{Tab}{Tab}{Tab}";
    private const string Tab6 = $"{Tab}{Tab}{Tab}{Tab}{Tab}{Tab}";
    
    public const string FilterCriteriaAttribute = @"namespace Common.Filtering;

/// <summary>
/// Mark a property as filterable and restrict allowed operators.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FilterCriteriaAttribute : Attribute
{
    /// <summary>
    /// Allowed operators for this field.
    /// </summary>
    public FilterOperator[] AllowedOperators { get; }

    public FilterCriteriaAttribute(params FilterOperator[] allowedOperators)
    {
        if (allowedOperators.Length == 0)
        {
            throw new ArgumentException(
                ""At least one FilterOperator must be specified"",
                nameof(allowedOperators));
        }

        AllowedOperators = allowedOperators;
    }
}";

    public const string FilterSpecificationAttribute = @"namespace Common.Filtering;

/// <summary>
/// Mark a class as filterable.
/// Only types with this attribute will have FilterSpec generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class FilterSpecificationAttribute : Attribute
{
}";

    public static string GenerateFilterSpecificationV2(FilterableObject target)
    {
        var sb = new StringBuilder();
        var className = $"{target.ClassName}FilterSpecification";
        
        sb.AppendLine("using Common.Filtering;");
        sb.AppendLine();
        sb.Append("namespace ").Append(target.Namespace).AppendLine(";");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();

        sb.Append("public class ").Append(className).Append(" : FilterSpecificationBase<").Append(target.ClassName).AppendLine(">");
        sb.AppendLine("{");

        foreach (var filterableProperty in target.FilterableProperties)
        {
            sb.Append(Tab).Append("public FilterDescriptor<").Append(filterableProperty.Type).Append(">? ").Append(filterableProperty.Name).AppendLine("Descriptor { get; internal set; }");
        }
        sb.Append(Tab).Append("public static ").Append(className).AppendLine(" Map(FilterRequest request)");
        sb.Append(Tab).AppendLine("{");
        sb.Append(Tab2).Append("var spec = new ").Append(className).AppendLine("();");
        sb.Append(Tab2).AppendLine("spec.Limit = request.Limit;");
        sb.Append(Tab2).AppendLine("spec.Offset = request.Offset;");

        sb.Append(Tab2).AppendLine("foreach (var criterion in request.Criteria)");
        sb.Append(Tab2).AppendLine("{");

        foreach (var filterableProperty in target.FilterableProperties)
        {
            // todo vm: add validation error property and fill it in this code part
            sb.Append(Tab3).Append("if (criterion.FieldName.Equals(\"").Append(filterableProperty.Name).AppendLine("\", StringComparison.InvariantCultureIgnoreCase))");
            sb.Append(Tab3).AppendLine("{");

            var allowedOperationsString = string.Join(" or ", filterableProperty.FilterableOperators.Select(x => $"(FilterOperator){x}"));
            sb.Append(Tab4).Append("if (criterion.Operator is not (").Append(allowedOperationsString).AppendLine("))");
            sb.Append(Tab5).AppendLine("continue;");

            // if value is parsable, check it too.
            if (filterableProperty.IsParsable)
            {
                // strings shouldnt be TryParsed
                if (filterableProperty.Type == "string")
                {
                    sb.Append(Tab4).Append("spec.").Append(filterableProperty.Name).Append("Descriptor = new FilterDescriptor<").Append(filterableProperty.Type).AppendLine(">(criterion.Value ,criterion.Operator);");
                }
                else
                {
                    sb.Append(Tab4).Append("if (").Append(filterableProperty.Type).AppendLine(".TryParse(criterion.Value, null, out var value))");
                    sb.Append(Tab5).Append("spec.").Append(filterableProperty.Name).Append("Descriptor = new FilterDescriptor<").Append(filterableProperty.Type).AppendLine(">(value ,criterion.Operator);");
                }
            }
            
            sb.Append(Tab4).AppendLine("spec.AddCriteria(criterion.FieldName, criterion.Operator, criterion.Value);");
            sb.Append(Tab4).AppendLine("continue;");
            sb.Append(Tab3).AppendLine("}");
        }
            
        sb.Append(Tab2).AppendLine("}");

        sb.Append(Tab2).AppendLine("return spec;");
        
        sb.Append(Tab).AppendLine("}");
        
        sb.AppendLine("}");

        return sb.ToString();
    }
}