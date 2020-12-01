using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fux.Core.Extension.PropertyBuilder
{
    /// <summary>
    /// This class maintains our JSON conversion extension for
    /// <code><![CDATA[Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<TProperty>]]></code>
    /// </summary>
    public static class HasJsonConversionExtension
    {
        /// <summary>
        /// This method bootstraps adding a JSON conversion to a
        /// <code><![CDATA[Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<TProperty>]]></code>
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(this PropertyBuilder<TProperty> source)
        {
            // Check the  type
            if (Reflection.IsSystemType<TProperty>())
                throw new Exception($"{nameof(TProperty)} Cannot Be System Type");
            // Add the conversion to the instance
            source.HasConversion(
                value => Convert.ToString<TProperty>(value),
                value => Convert.FromString<TProperty>(value)
            );
            // We're done, return the source property builder
            return source;
        }
    }
}
