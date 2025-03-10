using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace webapp.Pagination;

public class WeeklyPagination<T> : List<T> {
    public DateTime StartOfWeek { get; private set; }
    public DateTime EndOfWeek => StartOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

    public async Task PaginateAsync(IQueryable<T> source, DateTime referenceDate, Expression<Func<T, DateTime>> dateSelector) {
        StartOfWeek = GetStartOfWeek(referenceDate);
        var items = await source.Where(GetDateFilter(dateSelector))
                        .OrderBy(dateSelector)
                        .ToListAsync();
        Clear();
        AddRange(items);
    }

    private static DateTime GetStartOfWeek(DateTime date) {
        int diff = date.DayOfWeek - DayOfWeek.Sunday;
        return date.AddDays(-diff).Date;
    }

    private Expression<Func<T, bool>> GetDateFilter(Expression<Func<T, DateTime>> dateSelector) {
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                // referenceDate >= StartOfWeek
                Expression.GreaterThanOrEqual(dateSelector.Body, Expression.Constant(StartOfWeek)),
                // referenceDate <= StartOfWeek
                Expression.LessThanOrEqual(dateSelector.Body, Expression.Constant(EndOfWeek))
            ),
            dateSelector.Parameters
        );
    }
}