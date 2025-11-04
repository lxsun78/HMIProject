using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Widgets.Models
{
    public class CollectionBasedDictionary<TKey, TValue>  : KeyedCollection<TKey, KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        public CollectionBasedDictionary()
            : base()
        {
        }

        public CollectionBasedDictionary(IDictionary<TKey, TValue> dictionary)
            : base()
        {
            foreach (var kvp in dictionary)
            {
                Add(kvp);
            }
        }

        public CollectionBasedDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public CollectionBasedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            foreach (var kvp in dictionary)
            {
                Add(kvp);
            }
        }


        protected override TKey GetKeyForItem(KeyValuePair<TKey, TValue> item)
        {
            return item.Key;
        }



        public ICollection<TKey> Keys
        {
            get
            {
                if (Dictionary == null)
                {
                    return Enumerable.Empty<TKey>().ToArray();
                }
                return Dictionary.Keys;
            }
        }


        public ICollection<TValue> Values
        {
            get
            {
                if (Dictionary == null)
                {
                    return Enumerable.Empty<TValue>().ToArray();
                }
                return Dictionary.Values
                    .Select(x => x.Value)
                    .ToArray();
            }
        }

        public new TValue this[TKey key]
        {
            get
            {
                if (Dictionary == null)
                {
                    throw new KeyNotFoundException(nameof(Dictionary));
                }
                return Dictionary[key].Value;
            }
            set
            {
                var newValue = new KeyValuePair<TKey, TValue>(key, value);
                if (Dictionary.TryGetValue(key, out var oldValue))
                {
                    var index = Items.IndexOf(oldValue);
                    SetItem(index, newValue);
                }
                else
                {
                    InsertItem(Count, newValue);
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            var kvp = new KeyValuePair<TKey, TValue>(key, value);
            Add(kvp);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = Dictionary.TryGetValue(key, out var kvp);

            value = kvp.Value;
            return result;
        }
    }
}
