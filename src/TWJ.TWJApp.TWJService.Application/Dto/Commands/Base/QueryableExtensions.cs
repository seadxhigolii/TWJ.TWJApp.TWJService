using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Dto.Commands.Base
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy, string sortDirection, int? topRecords = null)
        {
            if (string.IsNullOrEmpty(sortBy) || string.IsNullOrEmpty(sortDirection))
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(parameter, sortBy.FirstCharToUpper());
            var lambda = Expression.Lambda(property, parameter);

            // Determine method names based on sortDirection and whether topRecords is specified
            string methodName = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "OrderBy" : "OrderByDescending";
            if (topRecords.HasValue && !string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase))
            {
                methodName += "Descending"; 
            }

            MethodCallExpression resultExpression = Expression.Call(typeof(Queryable), methodName,
                new Type[] { query.ElementType, property.Type },
                query.Expression, Expression.Quote(lambda));

            query = query.Provider.CreateQuery<T>(resultExpression);

            if (topRecords.HasValue)
            {
                query = query.Take(topRecords.Value);
            }

            return query;
        }

        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };
    }

}
