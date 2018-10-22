using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ZibaobaoLib.Helpers
{
    public static class CollectionExtension
    {
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                collection.Add(item);
            }
            return collection;
        }
    }
}