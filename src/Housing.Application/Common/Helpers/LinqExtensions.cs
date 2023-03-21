using System.Linq.Expressions;

namespace Housing.Application.Common.Helpers
{
    public static class LinqExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Func<bool> condition, Expression<Func<T, bool>> predicate)
        {
            if (condition())
            {
                return source.Where(predicate);
            }

            return source;
        }
    }
}
