using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Fg.EFCore.QueryExtensions
{
    public static class FilterExpression
    {
        public static Expression<Func<T, bool>> LikeOneOf<T>(string propertyName, IEnumerable<string> values)
        {
            var parameter = Expression.Parameter(typeof(T));

            var searchProperty = typeof(T).GetProperty(propertyName);

            if (searchProperty == null)
            {
                throw new ArgumentException($"Property {propertyName} not found in type {typeof(T).Name}");
            }

            var likeFunction = typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like), new[]{typeof(DbFunctions), typeof(string), typeof(string)} );

            if (likeFunction == null)
            {
                throw new InvalidOperationException($"Unable to find {nameof(DbFunctionsExtensions.Like)}");
            }

            var body = values.Select(value => Expression.Call(likeFunction,
                                                             Expression.Constant(EF.Functions),
                                                             Expression.Property(parameter, searchProperty),
                                                             Expression.Constant(value)))
                             .Aggregate<MethodCallExpression, Expression>(null, (current, call) => current != null ? Expression.OrElse(current, call) : (Expression)call);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
