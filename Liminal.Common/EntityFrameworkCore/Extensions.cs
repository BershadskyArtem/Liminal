using System.Runtime.CompilerServices;
using Liminal.Common.Requests;

namespace Liminal.Common.EntityFrameworkCore;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<T> Paged<T>(this IQueryable<T> query, PagedRequest req)
    {
        var skip = (req.PageNumber - 1) * req.PageSize;
        var take = req.PageSize;

        query = query.Skip(skip).Take(take);
        
        return query;
    }
}