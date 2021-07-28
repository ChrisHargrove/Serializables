using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    /// <summary>
    /// Base class for the SerializableDictionary that is primarily used to allow the
    /// PropertyDrawer to be assigned to the SerializedDictionary without generic arguments.
    /// </summary>
    [Serializable]
    public abstract class SerializableDictionaryBase
    { }

    /// <summary>
    /// Wrapper class that allows for the Serialization of a dictionary in the Inspector.
    /// <para>
    /// Allows for implicit conversion to and from a normal Dictionary.
    /// </para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : SerializableDictionaryBase, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct ValueWrapper
        {
            public TValue Value;

            public ValueWrapper(TValue value)
            {
                Value = value;
            }
        }

        public SerializableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        private Dictionary<TKey, TValue> dictionary;

        [SerializeField, HideInInspector]
        private TKey[] keys;
        [SerializeField, HideInInspector]
        private ValueWrapper[] values;

        #region Operators

        public static implicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> serializedDictionary)
            => serializedDictionary.dictionary;

        public static implicit operator SerializableDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
            => new SerializableDictionary<TKey, TValue>(dictionary);

        public static bool operator ==(SerializableDictionary<TKey, TValue> a, Dictionary<TKey, TValue> b) => a.dictionary == b;
        public static bool operator !=(SerializableDictionary<TKey, TValue> a, Dictionary<TKey, TValue> b) => a.dictionary != b;
        public static bool operator ==(Dictionary<TKey, TValue> a, SerializableDictionary<TKey, TValue> b) => a == b.dictionary;
        public static bool operator !=(Dictionary<TKey, TValue> a, SerializableDictionary<TKey, TValue> b) => a != b.dictionary;

        #endregion

        #region IDictionary

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)dictionary).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)dictionary).Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

        public TValue this[TKey key] { get => dictionary[key]; set => dictionary[key] = value; }

        public void Add(TKey key, TValue value)
             => dictionary.Add(key, value);

        public bool ContainsKey(TKey key)
            => dictionary.ContainsKey(key);

        public bool Remove(TKey key)
            => dictionary.Remove(key);

        public bool TryGetValue(TKey key, out TValue value)
            => dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item)
            => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);

        public void Clear()
            => dictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)dictionary).GetEnumerator();

        #endregion

        #region ISerializationCallbackReceiver

        public void OnAfterDeserialize()
        {
            if (keys != null && values != null)
            {
                if (dictionary == null) dictionary = new Dictionary<TKey, TValue>(keys.Length);
                else dictionary.Clear();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i < values.Length)
                        dictionary[keys[i]] = values[i].Value;
                    else
                        dictionary[keys[i]] = default(TValue);
                }
            }

            keys = null;
            values = null;
        }

        public void OnBeforeSerialize()
        {
            if (typeof(TKey).IsEnum)
            {
                foreach (TKey key in Enum.GetValues(typeof(TKey)))
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, default(TValue));
                    }
                }
            }

            if (dictionary == null || dictionary.Count == 0)
            {
                keys = null;
                values = null;
            }
            else
            {
                int count = dictionary.Count;
                keys = new TKey[count];
                values = new ValueWrapper[count];
                int i = 0;
                Dictionary<TKey, TValue>.Enumerator e = dictionary.GetEnumerator();
                while (e.MoveNext())
                {
                    keys[i] = e.Current.Key;
                    values[i] = new ValueWrapper(e.Current.Value);
                    i++;
                }
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is SerializableDictionary<TKey, TValue> serialized)
            {
                return this.dictionary == serialized.dictionary;
            }
            if (obj is Dictionary<TKey, TValue> dictionary)
            {
                return this.dictionary == dictionary;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return dictionary.GetHashCode();
        }

        public override string ToString()
        {
            return dictionary.ToString();
        }
    }
}