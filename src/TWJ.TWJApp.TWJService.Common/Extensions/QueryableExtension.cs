using NinjaNye.SearchExtensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Asp.Nappox.School.Common.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<T> SearchFromInput<T>(this IQueryable<T> query, string value, params Expression<Func<T, string>>[] stringProperties)
        {
            if (string.IsNullOrEmpty(value)) return query;

            var newQuery = query.Search(stringProperties);

            string[] searchParams = value.Trim().Split(" ").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToArray();

            return newQuery.StartsWith(searchParams);
        }

        public static IQueryable<T> SkipAndTake<T>(this IQueryable<T> query, int page, int pageSize, out int totalPages, out int totalItems)
        {
            var total = totalItems = query.Count();

            if (page <= 0 || pageSize <= 0)
            {
                totalPages = 1;
                return query.Skip(0);
            }

            else totalPages = (int)Math.Ceiling(total / (double)pageSize);

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
