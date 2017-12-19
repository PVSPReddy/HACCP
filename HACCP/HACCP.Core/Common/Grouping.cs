using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HACCP.Core
{
    public class Grouping<K, T> : ObservableCollection<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="items">Items.</param>
        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
                Items.Add(item);
        }

        public K Key { get; private set; }
    }
}