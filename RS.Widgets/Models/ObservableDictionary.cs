using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Widgets.Models
{

    public class ObservableDictionary<TKey, TValue> : CollectionBasedDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region 构造函数

        public ObservableDictionary() : base()
        {
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
        }
        #endregion 

        #region 字段
        private const string DictionaryName = "Dictionary";
        private const string ItemsName = "Items[]";
        private const string KeysName = "Keys[]";
        private const string ValuesName = "Values[]";
        private const string CountName = "Count";
        #endregion 

        #region 事件
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }
            remove
            {
                PropertyChanged -= value;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnCollectionAdded(KeyValuePair<TKey, TValue> changedItem, int startingIndex)
        {
            OnPropertyChanged(DictionaryName);
            OnPropertyChanged(ItemsName);
            OnPropertyChanged(CountName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItem, startingIndex));
        }

        private void OnCollectionRemoved(KeyValuePair<TKey, TValue> changedItem, int startingIndex)
        {
            OnPropertyChanged(DictionaryName);
            OnPropertyChanged(ItemsName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
            OnPropertyChanged(CountName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItem, startingIndex));
        }

        private void OnCollectionMoved(KeyValuePair<TKey, TValue> changedItem, int index, int oldIndex)
        {
            OnPropertyChanged(DictionaryName);
            OnPropertyChanged(ItemsName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, changedItem, index, oldIndex));
        }

        private void OnCollectionReplaced(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged(DictionaryName);
            OnPropertyChanged(ItemsName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));
        }

        private void OnCollectionReset()
        {
            OnPropertyChanged(DictionaryName);
            OnPropertyChanged(ItemsName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
            OnPropertyChanged(CountName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        #endregion 

        #region 方法
        protected override void SetItem(int index, KeyValuePair<TKey, TValue> item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);
            OnCollectionReplaced(item, oldItem);
        }

        protected override void InsertItem(int index, KeyValuePair<TKey, TValue> item)
        {
            base.InsertItem(index, item);
            OnCollectionAdded(item, index);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];
            base.RemoveItem(index);
            OnCollectionRemoved(removedItem, index);
        }


        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionReset();
        }
        #endregion
    }
}
