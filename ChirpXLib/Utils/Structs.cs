﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ChirpXLib.Utils
{
    public sealed class SizedDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

        private int maxSize;
        private Queue<TKey> keys;

        public SizedDictionary(int size)
        {
            maxSize = size;
            keys = new Queue<TKey>();
        }

        public new void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException();
            base.TryAdd(key, value);
            keys.Enqueue(key);
            if (keys.Count > maxSize) base.Remove(keys.Dequeue());
        }

        public new bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException();
            if (!keys.Contains(key)) return false;
            var newQueue = new Queue<TKey>();
            while (keys.Count > 0)
            {
                var thisKey = keys.Dequeue();
                if (!thisKey.Equals(key)) newQueue.Enqueue(thisKey);
            }
            keys = newQueue;
            return base.Remove(key);
        }
    }
    public sealed class SizedDictionaryAsync<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {

        private int maxSize;
        private Queue<TKey> keys;

        public SizedDictionaryAsync(int size)
        {
            maxSize = size;
            keys = new Queue<TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException();
            base.TryAdd(key, value);
            keys.Enqueue(key);
            if (keys.Count > maxSize) base.TryRemove(keys.Dequeue(), out _);
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException();
            if (!keys.Contains(key)) return false;
            var newQueue = new Queue<TKey>();
            while (keys.Count > 0)
            {
                var thisKey = keys.Dequeue();
                if (!thisKey.Equals(key)) newQueue.Enqueue(thisKey);
            }
            keys = newQueue;
            return base.TryRemove(key, out _);
        }
    }
}
