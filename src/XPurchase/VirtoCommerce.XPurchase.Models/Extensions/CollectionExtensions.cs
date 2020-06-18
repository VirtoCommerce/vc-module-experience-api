﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.XPurchase.Models.Enums;

namespace VirtoCommerce.XPurchase.Models.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add a range of items to a collection.
        /// </summary>
        /// <typeparam name="T">Type of objects within the collection.</typeparam>
        /// <param name="collection">The collection to add items to.</param>
        /// <param name="items">The items to add to the collection.</param>
        /// <returns>The collection.</returns>
        /// <exception cref="System.ArgumentNullException">An <see cref="System.ArgumentNullException"/> is thrown if <paramref name="collection"/> or <paramref name="items"/> is <see langword="null"/>.</exception>
		public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new System.ArgumentNullException("collection");
            if (items == null) throw new System.ArgumentNullException("items");

            foreach (var each in items)
            {
                collection.Add(each);
            }

            return collection;
        }

        public static void AddDistinct<T>(this ICollection<T> obj, params T[] items)
        {
            AddDistinct(obj, null, items);
        }

        public static void AddDistinct<T>(this ICollection<T> obj, IEqualityComparer<T> comparer, params T[] items)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (items != null)
            {
                foreach (var item in items)
                {
                    var contains = comparer != null ? obj.Contains(item, comparer) : obj.Contains(item);

                    if (!contains)
                        obj.Add(item);
                }
            }
        }


        public static void Replace<T>(this ICollection<T> obj, IEnumerable<T> newItems)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            obj.Clear();
            obj.AddRange(newItems);
        }

        public static void Patch<T>(this ICollection<T> source, ICollection<T> target, Action<T, T> patch)
        {
            source.Patch(target, EqualityComparer<T>.Default, patch);
        }


        public static void Patch<T>(this ICollection<T> source, ICollection<T> target, IEqualityComparer<T> comparer, Action<T, T> patch)
        {
            Action<EntryState, T, T> patchAction = (state, x, y) =>
            {
                if (state == EntryState.Modified)
                {
                    patch(x, y);
                }
                else if (state == EntryState.Added)
                {
                    target.Add(x);
                }
                else if (state == EntryState.Deleted)
                {
                    target.Remove(x);
                }
            };

            source.CompareTo(target, comparer, patchAction);
        }

        public static void CompareTo<T>(this ICollection<T> source, ICollection<T> target, IEqualityComparer<T> comparer, Action<EntryState, T, T> action)
        {
            //Change
            foreach (var sourceItem in source)
            {
                var targetItem = target.FirstOrDefault(x => comparer.Equals(x, sourceItem));
                if (targetItem != null && !targetItem.Equals(default(T)))
                {
                    action(EntryState.Modified, sourceItem, targetItem);
                }
            }
            //Add
            foreach (var newItem in source.Except(target, comparer))
            {
                action(EntryState.Added, newItem, newItem);
            }
            //Remove
            foreach (var removedItem in target.Except(source, comparer).ToArray())
            {
                action(EntryState.Deleted, removedItem, removedItem);
            }
        }



    }
}
