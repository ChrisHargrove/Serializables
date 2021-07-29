using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BatteryAcid.Serializables
{
    [Serializable]
    public abstract class SerializableHashSetBase
    { }

    [Serializable]
    public class SerializableHashSet<TValue> : SerializableHashSetBase, IReadOnlyCollection<TValue>, ISet<TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class ValueWrapper
        {
            public TValue Value;

            public ValueWrapper(TValue value)
            {
                Value = value;
            }
        }

        public SerializableHashSet()
            => hashset = new HashSet<TValue>();

        public SerializableHashSet(IEnumerable<TValue> enumerable)
            => hashset = new HashSet<TValue>(enumerable);

        public SerializableHashSet(IEqualityComparer<TValue> comparer)
            => hashset = new HashSet<TValue>(comparer);

        public SerializableHashSet(IEnumerable<TValue> enumerable, IEqualityComparer<TValue> comparer)
            => hashset = new HashSet<TValue>(enumerable, comparer);

        private HashSet<TValue> hashset;

        [SerializeField, HideInInspector]
        private List<ValueWrapper> values;

        public IEqualityComparer<TValue> Comparer => hashset.Comparer;

        #region Operators

        public static implicit operator HashSet<TValue>(SerializableHashSet<TValue> serialized)
            => serialized.hashset;

        public static implicit operator SerializableHashSet<TValue>(HashSet<TValue> hashset)
            => new SerializableHashSet<TValue>(hashset);

        #endregion

        #region IReadOnlyCollection

        public int Count => hashset.Count;

        public bool IsReadOnly => ((ICollection<TValue>)hashset).IsReadOnly;

        public void Add(TValue item)
        {
            ((ICollection<TValue>)hashset).Add(item);
        }

        public void Clear()
        {
            hashset.Clear();
        }

        public bool Contains(TValue item)
        {
            return hashset.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            hashset.CopyTo(array, arrayIndex);
        }

        public void CopyTo(TValue[] array, int arrayIndex, int count)
        {
            hashset.CopyTo(array, arrayIndex, count);
        }

        public void CopyTo(TValue[] array)
        {
            hashset.CopyTo(array);
        }

        public bool Remove(TValue item)
        {
            return hashset.Remove(item);
        }

        public int RemoveWhere(Predicate<TValue> match)
        {
            return hashset.RemoveWhere(match);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return ((IEnumerable<TValue>)hashset).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)hashset).GetEnumerator();
        }

        #endregion

        #region ISet

        bool ISet<TValue>.Add(TValue item)
        {
            return hashset.Add(item);
        }

        public void ExceptWith(IEnumerable<TValue> other)
        {
            hashset.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<TValue> other)
        {
            hashset.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<TValue> other)
        {
            return hashset.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<TValue> other)
        {
            return hashset.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<TValue> other)
        {
            return hashset.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<TValue> other)
        {
            return hashset.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<TValue> other)
        {
            return hashset.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<TValue> other)
        {
            return hashset.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<TValue> other)
        {
            hashset.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<TValue> other)
        {
            hashset.UnionWith(other);
        }

        public void TrimExcess()
        {
            hashset.TrimExcess();
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
            if (hashset == null)
            {
                values = null;
            }
            else
            {
                values = new List<ValueWrapper>(hashset.Count);
                HashSet<TValue>.Enumerator enumerator = hashset.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    values.Add(new ValueWrapper(enumerator.Current));
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (values != null)
            {
                if (hashset == null) hashset = new HashSet<TValue>();
                else hashset.Clear();

                for (int i = 0; i < values.Count; i++)
                    hashset.Add(values[i].Value);
            }
        }

        #endregion
    }
}
