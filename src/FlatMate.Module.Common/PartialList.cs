using System;
using System.Collections.Generic;
using System.Linq;
using FlatMate.Module.Common.Api;

namespace FlatMate.Module.Common
{
    public static class PartialListExtension
    {
        public static PartialList<TOut> MapTo<T, TOut>(this PartialList<T> partialList, Func<T, TOut> mapFunc)
        {
            return new PartialList<TOut>(partialList.Items.Select(mapFunc), partialList.Limit, partialList.Offset, partialList.Total);
        }
    }

    public class PartialList<T>
    {
        public PartialList(IEnumerable<T> items, int limit, int offset, int total)
        {
            Items = items;
            Limit = limit;
            Offset = offset;
            Total = total;
        }

        public PartialList(IEnumerable<T> items, PartialListParameter parameter, int total)
        {
            Items = items;
            Limit = parameter.Limit;
            Offset = parameter.Offset;
            Total = total;
        }

        public IEnumerable<T> Items { get; }

        public int Limit { get; }

        public int Offset { get; }

        public int Total { get; }
    }
}