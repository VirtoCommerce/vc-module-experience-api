using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class ExtendableGraphType<TSourceType> : ObjectGraphType<TSourceType>
    {
        public FieldType ExtendableField<TGraphType>(
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, object> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            return AddField(new FieldType
            {
                Name = name,
                Description = description,
                DeprecationReason = deprecationReason,
                Type = GraphTypeExtenstionHelper.GetActualComplexType<TGraphType>(),
                Arguments = arguments,
                Resolver = resolve != null
                    ? new FuncFieldResolver<TSourceType, object>(context =>
                        {
                            context.CopyArgumentsToUserContext();
                            return resolve(context);
                        })
                    : null
            });
        }

        public FieldType ExtendableFieldAsync<TGraphType>(
          string name,
          string description = null,
          QueryArguments arguments = null,
          Func<IResolveFieldContext<TSourceType>, Task<object>> resolve = null,
          string deprecationReason = null)
          where TGraphType : IGraphType
        {
            return AddField(new FieldType
            {
                Name = name,
                Description = description,
                DeprecationReason = deprecationReason,
                Type = GraphTypeExtenstionHelper.GetActualComplexType<TGraphType>(),
                Arguments = arguments,
                Resolver = resolve != null
                    ? new FuncFieldResolver<TSourceType, Task<object>>(context =>
                    {
                        context.CopyArgumentsToUserContext();
                        return resolve(context);
                    })
                    : null
            });
        }

        public void LocalizedField(Expression<Func<TSourceType, string>> expression, SettingDescriptor descriptor, ILocalizableSettingService localizableSettingService, bool nullable)
        {
            // Add original field
            Field(expression, nullable);

            // Add localized field
            var getValue = expression.Compile();
            var localizedFieldName = expression.NameOf().ToCamelCase() + "DisplayValue";

            if (nullable)
            {
                Field<StringGraphType>(localizedFieldName, resolve: context =>
                    localizableSettingService.TranslateAsync(getValue(context.Source), descriptor.Name, context.GetCultureName()));
            }
            else
            {
                Field<NonNullGraphType<StringGraphType>>(localizedFieldName, resolve: context =>
                    localizableSettingService.TranslateAsync(getValue(context.Source), descriptor.Name, context.GetCultureName()));
            }
        }
    }
}
