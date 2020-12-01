using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fux.Core.Extension.PropertyBuilder;

namespace Fux.Core.Extension.EntityTypeBuilder
{
    /// <summary>
    /// This class maintains our JSON conversion extension for
    /// <code><![CDATA[Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TProperty>]]></code>
    /// </summary>
    public static class JsonPropertyExtension
    {
        /// <summary>
        /// This method bootstraps adding a JSON conversion to a
        /// <code><![CDATA[Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity>]]></code>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static EntityTypeBuilder<TEntity> JsonProperty<TEntity, TProperty>(this EntityTypeBuilder<TEntity> source,
            [NotNullAttribute] Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity: class, new() where TProperty: class, new()
        {
            // Set the conversion on the property
            source.Property<TProperty>(propertyExpression)
                .HasJsonConversion();
            // We're done, return the source entity type builder
            return source;
        }
    }
}
