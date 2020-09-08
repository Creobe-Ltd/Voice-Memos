using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Creobe.VoiceMemos.Core.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddToCollection<T>(this ObservableCollection<T> collection, T item)
        {
            if (!collection.Contains(item))
                collection.Add(item);
        }

        public static void AddToCollection<T, TKey>(this ObservableCollection<T> collection, T item, Func<T, TKey> keySelector)
        {
            collection.AddToCollection(item, keySelector, ListSortDirection.Ascending);
        }

        public static void AddToCollection<T, TKey>(this ObservableCollection<T> collection, T item, Func<T, TKey> keySelector, ListSortDirection direction)
        {
            collection.AddToCollection(item);

            if (direction == ListSortDirection.Ascending)
                collection.Sort(keySelector);
            else
                collection.SortDescending(keySelector);
        }

        public static void AddRangeToCollection<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.AddToCollection(item);
            }
        }

        public static void AddRangeToCollection<T, TKey>(this ObservableCollection<T> collection, IEnumerable<T> items, Func<T, TKey> keySelector)
        {
            collection.AddRangeToCollection(items, keySelector, ListSortDirection.Ascending);
        }

        public static void AddRangeToCollection<T, TKey>(this ObservableCollection<T> collection, IEnumerable<T> items, Func<T, TKey> keySelector, ListSortDirection direction)
        {
            collection.AddRangeToCollection(items);

            if (direction == ListSortDirection.Ascending)
                collection.Sort(keySelector);
            else
                collection.SortDescending(keySelector);
        }

        public static void RemoveFromCollection<T>(this ObservableCollection<T> collection, T item)
        {
            if (collection.Contains(item))
                collection.Remove(item);
        }

        public static void RemoveFromCollection<T, TKey>(this ObservableCollection<T> collection, T item, Func<T, TKey> keySelector)
        {
            collection.RemoveFromCollection(item, keySelector, ListSortDirection.Ascending);
        }

        public static void RemoveFromCollection<T, TKey>(this ObservableCollection<T> collection, T item, Func<T, TKey> keySelector, ListSortDirection direction)
        {
            collection.RemoveFromCollection(item);

            if (direction == ListSortDirection.Ascending)
                collection.Sort(keySelector);
            else
                collection.SortDescending(keySelector);
        }

        public static void RemoveRangeFromCollection<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.RemoveFromCollection(item);
            }
        }

        public static void RemoveRangeFromCollection<T, TKey>(this ObservableCollection<T> collection, IEnumerable<T> items, Func<T, TKey> keySelector)
        {
            collection.RemoveRangeFromCollection(items, keySelector, ListSortDirection.Ascending);
        }

        public static void RemoveRangeFromCollection<T, TKey>(this ObservableCollection<T> collection, IEnumerable<T> items, Func<T, TKey> keySelector, ListSortDirection direction)
        {
            collection.RemoveRangeFromCollection(items);

            if (direction == ListSortDirection.Ascending)
                collection.Sort(keySelector);
            else
                collection.SortDescending(keySelector);
        }

        public static void Sort<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector)
        {
            var sortedItems = collection.OrderBy(keySelector).ToList();

            foreach (var item in sortedItems)
            {
                collection.Move(collection.IndexOf(item), sortedItems.IndexOf(item));
            }

            sortedItems = null;
        }

        public static void SortDescending<T, TKey>(this ObservableCollection<T> collection, Func<T, TKey> keySelector)
        {
            var sortedItems = collection.OrderByDescending(keySelector).ToList();

            foreach (var item in sortedItems)
            {
                collection.Move(collection.IndexOf(item), sortedItems.IndexOf(item));
            }

            sortedItems = null;
        }

    }
}
