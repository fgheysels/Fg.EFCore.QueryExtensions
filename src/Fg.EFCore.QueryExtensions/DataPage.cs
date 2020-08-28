using System;
using System.Collections.Generic;

namespace Fg.EFCore.QueryExtensions
{
    public class DataPage<T>
    {
        public int TotalNumberOfItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public IEnumerable<T> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPage"/> class.
        /// </summary>
        public DataPage(IEnumerable<T> items, int totalNumberOfItems, int pageNumber, int pageSize)
        {
            Items = items;
            TotalNumberOfItems = totalNumberOfItems;
            PageNumber = pageNumber;
            PageSize = pageSize;

            if (totalNumberOfItems == 0)
            {
                TotalPages = 1;
            }
            else
            {
                TotalPages = (int)Math.Ceiling(TotalNumberOfItems / (double)pageSize);
            }
        }
    }
}
