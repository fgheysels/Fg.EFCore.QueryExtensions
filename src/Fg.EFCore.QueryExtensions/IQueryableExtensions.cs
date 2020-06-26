using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Fg.EFCore.QueryExtensions
{
    public static class IQueryableExtensions
    {
        public static async Task<DataPage<T>> ToPagedResult<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            int numberOfItems = await source.CountAsync();

            int numberOfItemsToSkip = (pageNumber - 1) * pageSize;

            var items = await source.Skip(numberOfItemsToSkip).Take(pageSize).ToListAsync();

            return new DataPage<T>(items, numberOfItems, pageNumber, pageSize);
        }
    }
}