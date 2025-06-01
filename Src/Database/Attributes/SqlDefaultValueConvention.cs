using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Reflection;


namespace SlugEnt.BWA.Database;

/// <summary>
/// Enables the ability to set a default value for a column in the database in the modelbuilder.
/// </summary>
public class SqlDefaultValueConvention
: PropertyAttributeConventionBase<SqlDefaultValueAttribute>
{
    /// <summary>
    /// Constructor for SQL Default Value Convention
    /// </summary>
    /// <param name="dependencies"></param>
    public SqlDefaultValueConvention(ProviderConventionSetBuilderDependencies dependencies)
        : base(dependencies) { }


    /// <summary>
    /// Process the property added to the model
    /// </summary>
    /// <param name="propertyBuilder"></param>
    /// <param name="attribute"></param>
    /// <param name="clrMember"></param>
    /// <param name="context"></param>
    protected override void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        SqlDefaultValueAttribute attribute,
        MemberInfo clrMember,
        IConventionContext context)
    {
        propertyBuilder.HasDefaultValueSql(attribute.DefaultValue);
    }
}
